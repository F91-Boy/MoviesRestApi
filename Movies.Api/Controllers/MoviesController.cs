using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController(IMovieRepository _movieRepository) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
        {
            var movie = new Movie {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genders = request.Genders.ToList()
            };
            await _movieRepository.CreateAsync(movie);
            return Created($"/api/movies/{movie.Id}",movie);
        }
    }
} 
