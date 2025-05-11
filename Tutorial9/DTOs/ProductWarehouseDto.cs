using System.ComponentModel.DataAnnotations;

namespace Tutorial9.DTOs;

public class ProductWarehouseDto
{
    [Required]
    public string IdProduct { get; set; }
    
    [Required]
    public string IdWarehouse { get; set; }
    
    
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}