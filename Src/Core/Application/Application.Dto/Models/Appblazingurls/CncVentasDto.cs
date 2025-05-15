using System.ComponentModel.DataAnnotations;

namespace Application.Dto.Models.Appblazingurls;

public class CncVentasDto
{

    [Key]
    public int IdcncVentas { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }


}
