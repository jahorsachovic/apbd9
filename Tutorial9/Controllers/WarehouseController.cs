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
    public async Task<IActionResult> addProductToWarehouseAsync([FromBody] ProductWarehouseDto data, CancellationToken cancellationToken)
    {

        try
        {
            var result = await _warehouseService.insertIntoProductWarehouseAsync(data, cancellationToken);
            
            return Ok(result);
            
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
        
    }
    
    
}