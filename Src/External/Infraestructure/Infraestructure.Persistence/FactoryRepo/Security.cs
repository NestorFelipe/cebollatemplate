using Application.Contracts.Commons;
using Microsoft.AspNetCore.Http;

namespace Infraestructure.Persistence.FactoryRepo;

public class Security : ISecurity
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    public Security(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserName()
    {
        var userName = _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;        
        return userName ?? "sistema";
    }
}
