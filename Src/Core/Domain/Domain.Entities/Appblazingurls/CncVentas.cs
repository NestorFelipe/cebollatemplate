using Domain.Entities.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Appblazingurls;

[Table("cnc_ventas", Schema = "dbo")]
public class CncVentas : AuditableBaseEntity
{
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }
}
