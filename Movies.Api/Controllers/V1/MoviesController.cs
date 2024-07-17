using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V1
{
    [ApiVersion("1.0",Deprecated = false)]
    [ApiController]
    public class MoviesController(IMovieService _movieService,IOutputCacheStore _outputCacheStore) : ControllerBase
    {
        //[ServiceFilter(typeof(ApiKeyAuthFilter))]
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.V1.Movies.Create)]
        [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.MapToMovie();
            await _movieService.CreateAsync(movie, token);

            await _outputCacheStore.EvictByTagAsync("movies",token);//令缓存失效
            
            var response = movie.MapToResponse();
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
        }

        [HttpGet(ApiEndpoints.V1.Movies.Get)]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration =30,VaryByHeader = "Accept,Accept-Encoding",Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, userId, token)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, token);

            if (movie is null)
            {
                return NotFound();
            }

            var response = movie.MapToResponse();
            return Ok(response);
        }

        [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 30, VaryByQueryKeys = ["title","yearOfRelease","sortBy","page","pageSize"],VaryByHeader = "Accept,Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MovieResponse),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var options = request.MapToOptions().WithUserId(userId);

            var movies = await _movieService.GetAllAsync(options, token);
            var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

            //var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);
            var moviesResponse = movies.MapToResponse(request.Page.Value, request.PageSize.Value, movieCount);
            return Ok(moviesResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.V1.Movies.Update)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [
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

            await _outputCacheStore.EvictByTagAsync("movies", token);//令缓存失效
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (!deleted)
            {
                return NotFound();
            }

            await _outputCacheStore.EvictByTagAsync("movies", token);//令缓存失效
            return Ok();
        }

    }
}
