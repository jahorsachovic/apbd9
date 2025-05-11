using Tutorial9.Model;
using Tutorial9.DTOs;

namespace Tutorial9.Services;

public interface IWarehouseService
{
    Task<int> AddProductWarehouseAsync(AddProductWarehouseDto data, CancellationToken cancellationToken);
}