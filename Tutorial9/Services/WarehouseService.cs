using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.Model;
using Tutorial9.DTOs;
using Tutorial9.Exceptions;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private readonly string _connectionString;

    public WarehouseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }
    
    public async Task<int> AddProductWarehouseAsync(AddProductWarehouseDto data, CancellationToken cancellationToken)
    {
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        //Check if a Product or Warehouse exist
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                                    SELECT * FROM Product WHERE IdProduct = @IdProduct;
                                    SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse;
                                    ";

            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("@IdWarehouse", data.IdWarehouse);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new ProductIdException($"Invalid Product id: {data.IdProduct}");
            }

            await reader.NextResultAsync();

            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new WarehouseIdException($"Invalid Warehouse id: {data.IdWarehouse}");
            }
            
            reader.Close();
        }
        
        //Check if data is not less than 0
        if (data.Amount <= 0)
            throw new AmountException("Amount less than zero.");
        
        //Check if there exists an Order that matches the request
        int orderId;
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = @"
                                    SELECT * FROM [Order]
                                    WHERE [Order].IdProduct = @IdProduct AND [Order].Amount = @Amount AND [Order].CreatedAt < @CreatedAt;
                                    ";
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result == null)
            {
                throw new Exception("No matching order present.");
            }
            
            orderId = (int)result;
        }
        
        
        //Check if the order has been completed already
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = "SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder;";
            command.Parameters.AddWithValue("@IdOrder", orderId);
            
            if (await command.ExecuteScalarAsync(cancellationToken) != null)
                throw new Exception("This order has already been fulfilled.");
        }
        
        //Update FulfilledAt column of the order with the current date and time
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder;";
            command.Parameters.AddWithValue("@IdOrder", orderId);
            
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        
        //Retrieve a price of the product
        decimal pricePerUnit;
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct;";
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            
            
            var priceResult = await command.ExecuteScalarAsync(cancellationToken);
            pricePerUnit = (decimal)priceResult;
        }
        
        //Insert a record (data.price = Product.price * data.Amount), CreatedAt = current time
        int newRowId;
        DbTransaction transaction = await connection.BeginTransactionAsync();
        await using (SqlCommand command = new SqlCommand())
        {
            command.Connection = connection;
            command.Transaction = transaction as SqlTransaction;
            command.CommandText = @"
            INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
            OUTPUT INSERTED.IdProductWarehouse
            VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, GETDATE());";
            command.Parameters.AddWithValue("@IdWarehouse", data.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("IdOrder", orderId);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@Price", data.Amount * pricePerUnit);

            try
            {
                newRowId = (int)await command.ExecuteScalarAsync(cancellationToken);
                await transaction.CommitAsync();
                
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        //Return the value of the primary key generated for the record inserted into Product_Warehouse
        return newRowId;
    }
}