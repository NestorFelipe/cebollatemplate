namespace Application.Dto.Commons;

public class ApiSettings
{
    public string? BaseUrl { get; set; } = string.Empty;
    public string? ApiUrl { get; set; } = string.Empty;
    public int IdcUser { get; set; }
}

public class ResponseToken {
    public string? tipoRespuesta { get; set; }
    public string? mensaje { get; set; }

}


public class RequestToken
{
    public string? token { get; set; }
}