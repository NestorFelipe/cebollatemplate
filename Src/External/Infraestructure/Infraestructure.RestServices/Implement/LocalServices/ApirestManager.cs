using Application.Contracts.Commons;
using Application.Contracts.Contracts.RestServices;
using Application.Dto.Commons;
using Microsoft.Extensions.Options;

namespace Infraestructure.RestServices.Implement.LocalServices;

public class ApirestManager : IApirestManager
{ 

    private readonly IOptions<ApiSettings> _apisetteings;
    private readonly IRestBuilder _restBuilder;
    public ApirestManager(IOptions<ApiSettings> apisetteings, IRestBuilder restBuilder)
    {
        _apisetteings = apisetteings;
        _restBuilder = restBuilder;
        _restBuilder.Urlbuilder(_apisetteings.Value.BaseUrl!, _apisetteings.Value.ApiUrl!);
    }

    public IRestBuilder PostAnonymous()
    {
        return _restBuilder
                    .Method(HttpMethod.Post);
    }

   
}
