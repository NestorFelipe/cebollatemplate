namespace Application.Dto.Models.Appsgp;

public record CncProgramaDto
{
    public string Programa { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}


public record CncProgramaCarteraDto : CncProgramaDto
{
    public ICollection<CarCarteraDto> Carteras { get; set; } = new List<CarCarteraDto>();
}
