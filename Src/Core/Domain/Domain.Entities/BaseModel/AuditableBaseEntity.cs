using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.BaseModel;

public class AuditableBaseEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificacion { get; set; }
}
