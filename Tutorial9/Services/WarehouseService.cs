using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.Model;
using Tutorial9.DTOs;

namespace Tutorial9.Services;

public class WarehouseService : IWarehouseService
{
    private readonly string _connectionString;

    public WarehouseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }
    
    public async Task<int> insertIntoProductWarehouseAsync(ProductWarehouseDto data, CancellationToken cancellationToken)
    {
        if (data.Amount <= 0)
            throw new Exception("Amount must be greater than zero.");
        
        var query = @"";
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        

        DbTransaction transaction = await connection.BeginTransactionAsync();
        
        

        return 0;

    }
}