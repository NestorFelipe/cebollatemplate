using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.Contracts.Appsgp;
using Application.Contracts.Commons;

namespace Presentation.Api.Authentication;

public class JwtAuthFilter : IAsyncAuthorizationFilter
{
    private readonly IManagerSession _acount;
    private readonly ISecurity _security;
    public JwtAuthFilter(IManagerSession acount, ISecurity security)
    {
        _acount = acount;
        _security = security;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var vToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();


            //if (vToken == null || !vToken.StartsWith(JwtBearerDefaults.AuthenticationScheme, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    context.Result = new UnauthorizedObjectResult("Token de autorizacion invalida");
            //    return;
            //}

            //var vResult = await _security.ValidaToken(vToken!.Replace(JwtBearerDefaults.AuthenticationScheme, "", StringComparison.InvariantCultureIgnoreCase).Trim());
            //if (vResult.Estado != State.Success)
            //{
            //    context.Result = new UnauthorizedObjectResult("Token de autorizacion invalida");
            //    return;
            //}

            //if (!((vResult.Objeto as TokenValidationResult)!.IsValid))
            //{
            //    context.Result = new UnauthorizedObjectResult("Token de autorizacion invalida");
            //    return;
            //}

            //context.HttpContext.User = new ClaimsPrincipal((vResult.Objeto as TokenValidationResult)!.ClaimsIdentity);
        }
        catch (Exception e)
        {
            context.Result = new UnauthorizedObjectResult($"Error : {e.Message}");
            return;
        }
    }
}
