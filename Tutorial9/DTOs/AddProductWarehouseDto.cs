using System.ComponentModel.DataAnnotations;

namespace Tutorial9.DTOs;

public class AddProductWarehouseDto
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Not a valid int number.")]
    public int Amount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}