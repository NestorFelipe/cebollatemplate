using Application.Contracts.Commons;
using Infraestructure.Mappers.Interfaces;

namespace Infraestructure.Mappers.Implements;

public class GValidators : IGValidators
{

    private readonly IValidFactory _valid;
    public bool IsValid { get; set; } = false;

    public GValidators(IValidFactory valid)
    {
        _valid = valid;
    }

    public async Task<string> RunAsync<T>(T ent) where T : class
    {            
        var valid = await _valid.GetValidator<T>().ValidateAsync(ent);
        IsValid = valid.IsValid;
        return valid.ToString(";");
    }
}
