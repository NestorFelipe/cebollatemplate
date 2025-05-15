using System.ComponentModel.DataAnnotations;
namespace Application.Dto.Models.Appsgp;

public record CarCarteraDto
{
    public DateTime FechaRegistro { get; set; }
    public int CncProgramaId { get; set; }
    public string? CodigoCartera { get; set; }
    public string? Descripcion { get; set; }
    public string? Observacion { get; set; }
}
