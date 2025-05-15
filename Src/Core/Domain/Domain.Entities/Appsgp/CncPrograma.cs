using Domain.Entities.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Appsgp;

[Table("cnc_programa", Schema = "public")]
public class CncPrograma : AuditableBaseEntity
{
    public string Programa { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;


    // Propiedad de navegación para la relación uno a muchos
    [InverseProperty("Programa")]
    public virtual ICollection<CarCartera> Carteras { get; set; } = new List<CarCartera>();

}
