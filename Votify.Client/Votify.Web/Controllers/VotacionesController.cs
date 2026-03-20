using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Votify.Services.Interfaces;
using Votify.Services.Models;

namespace Votify.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotacionesController:ControllerBase
    {
        private readonly IPopularService _popularService;
        public VotacionesController(IPopularService popularService)
        {
            _popularService = popularService;
        }
        [HttpPost("popular")]
        public async Task<IActionResult> CrearVotacionPopular([FromBody]CrearVotacionPopularRequest request)
        { try
            {
                var result = await _popularService.CrearVotacionPopularAsync(request);
                return Ok(result);

            }catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            }
    }
}
