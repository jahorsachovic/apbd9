using Microsoft.AspNetCore.Mvc;
using Tutorial9.Services;
using Tutorial9.Model;
using Tutorial9.DTOs;

namespace Tutorial9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouseAsync(AddProductWarehouseDto data, CancellationToken cancellationToken)
    {

        try
        {
            var entryId = await _warehouseService.AddProductWarehouseAsync(data, cancellationToken);
            
            return Ok(entryId);
            
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
        
    }
    
    
}