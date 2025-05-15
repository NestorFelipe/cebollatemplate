using Domain.Entities.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Appblazingurls;


[Table("cnc_ventasdetalle", Schema = "dbo")]
public class CncVentasdetalle : AuditableBaseEntity
{   
    public int CncVentasId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }  
}
