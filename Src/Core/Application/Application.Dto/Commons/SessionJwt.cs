namespace Application.Dto.Commons;


public class SessionJwt
{
    public int IdgenOrigentransaccion { get; set; }
    public int IdcOrigen { get; set; }
    public string TipoOrigen { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UniqueName { get; set; } = string.Empty;
    public string NameID { get; set; } = string.Empty;
    public string KeySecret { get; set; } = string.Empty;

}
