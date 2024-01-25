using CubeInCube.Backend.Contracts;
using CubeInCube.Backend.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CubeInCube.Backend.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CaseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        public async Task<ActionResult<CaseDto>> CreateCase(CaseForCreationDto caseForCreationDto)
        {
            var result = _serviceManager.CaseService.CreateCase(caseForCreationDto);

            return Ok(result);
        }
    }
}
