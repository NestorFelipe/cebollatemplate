

using Application.Dto.Enums;
using System.Net;

namespace Application.Dto.Commons;

public record class ResponseAction(
        State Estado = State.NoData,
        string? Message = null,
        object? Objeto = null,
        string? JwtToken = null,
        int Httpcode = (int)HttpStatusCode.NotFound,
        int? Id = null)
{
    public State Estado { get; set; } = Estado;
    public string? Message { get; set; } = Message;
    public object? Objeto { get; set; } = Objeto;
    public string? JwtToken { get; set; } = JwtToken;
    public int Httpcode { get; set; } = Httpcode;
    public int? Id { get; set; }
}