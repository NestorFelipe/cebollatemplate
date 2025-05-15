using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Presentation.Api.Controllers.Commons;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public abstract class AuthBaseController : ControllerBase
{



}
