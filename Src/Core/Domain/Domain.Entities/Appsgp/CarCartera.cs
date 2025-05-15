
using Domain.Entities.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entities.Appsgp;

[Table("car_cartera", Schema = "public")]
public class CarCartera : AuditableBaseEntity
{  
    public DateTime FechaRegistro { get; set; }


    [ForeignKey("Programa")]
    public int CncProgramaId { get; set; }
    public string? CodigoCartera { get; set; }
    public string? Descripcion { get; set; }
    public string? Observacion { get; set; }

    
    public virtual CncPrograma Programa { get; set; } = null!;

}
