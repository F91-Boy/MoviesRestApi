using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{

    [ApiController]
    public class RatingsController(IRatingService _ratingService) : ControllerBase
    {
        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, 
            [FromBody]RateMovieRequest request,CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var result = await _ratingService.RateMovieAsync(id,request.Rating,userId!.Value,token);
            return result ? Ok() : NotFound();
        }
    }
}
