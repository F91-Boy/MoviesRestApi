using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers.V2
{
    [ApiVersion("2.0",Deprecated = true)]
    [ApiController]
    public class MoviesController(IMovieService _movieService) : ControllerBase
    {
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.V2.Movies.Create)]
        public async Task<IActionResult> Create([FromBody]CreateMovieRequest request,CancellationToken token)
        {
            var movie = ContractMapping.MapToMovie(request);
            await _movieService.CreateAsync(movie, token);
            return CreatedAtAction(nameof(Get), new {idOrSlug = movie.Id},movie);
        }

        [HttpGet(ApiEndpoints.V2.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, userId, token)
                :await _movieService.GetBySlugAsync(idOrSlug, userId, token);

            if (movie is null)
            {
                return NotFound();
            }

            var response = movie.MapToResponse();
            return Ok(response);   
        }

        [HttpGet(ApiEndpoints.V2.Movies.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery]GetAllMoviesRequest request,CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var options = request.MapToOptions().WithUserId(userId);

            var movies = await _movieService.GetAllAsync(options, token);
            var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

            var moviesResponse = movies.MapToResponse(request.Page,request.PageSize,movieCount);
            return Ok(moviesResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.V2.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [
            FromBody]UpdateMovieRequest request, CancellationToken token) 
        {
            var userId = HttpContext.GetUserId();

            var movie = request.MapToMovie(id);
            var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
            if (updatedMovie is null)
            {
                return NotFound();
            }
            var response = movie.MapToResponse();
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.V2.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }

    }
} 
