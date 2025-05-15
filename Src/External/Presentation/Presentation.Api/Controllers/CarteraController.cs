using Application.Contracts.Contracts.Appsgp;
using Application.Dto.Commons;
using Application.Dto.Models.Appsgp;
using Domain.Entities.Appsgp;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Controllers.Commons;

namespace Presentation.Api.Controllers;

public class CarteraController : AuthBaseController
{

    protected readonly IAppsgpGenericCrud _appsgpGenericCrud;

    public CarteraController(IAppsgpGenericCrud appsgpGenericCrud)
    {
        _appsgpGenericCrud = appsgpGenericCrud;
    }

    [HttpPost("programa")]
    public async Task<ResponseAction> SavePrograma([FromBody] CncProgramaDto programaDto)
    {
        return await _appsgpGenericCrud.SaveEntity<CncPrograma, CncProgramaDto>(programaDto);
    }

    [HttpPost("programacartera")]
    public async Task<ResponseAction> SaveProgramaCartera([FromBody] CncProgramaCarteraDto programaDto)
    {
        return await _appsgpGenericCrud.SaveEntity<CncPrograma, CncProgramaCarteraDto>(programaDto);
    }

    [HttpPut("programa/{id}")]
    public async Task<ResponseAction> UpdatePrograma([FromBody] CncProgramaDto programaDto, [FromRoute] int id)
    {
        return await _appsgpGenericCrud.UpdateEntity<CncPrograma, CncProgramaDto>(programaDto, id);
    }

    [HttpDelete("programa/{id}")]
    public async Task<ResponseAction> DeletePrograma([FromRoute] int id)
    {
        return await _appsgpGenericCrud.DeleteEntity<CncPrograma>(id);
    }

    [HttpGet("programa")]
    public async Task<ResponseAction> GetAllProgramas()
    {      
        return await _appsgpGenericCrud.GetWhereEntity<CncPrograma, CncProgramaDto>();
    }

    [HttpGet("programa/{id}")]
    public async Task<ResponseAction> GetByID([FromRoute] int id)
    {
        return await _appsgpGenericCrud.GetById<CncPrograma, CncProgramaDto>(id);
    }

}
