﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Api.Mapping;
using Asp.Versioning;
using System.Collections;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RatingsController(IRatingService _ratingService) : ControllerBase
    {
        [Authorize]
        [HttpPut(ApiEndpoints.V1.Movies.Rate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id,
            [FromBody] RateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            //var result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
            var result = await _ratingService.RateMovieAsync(id, request.Rating.GetValueOrDefault(5), userId!.Value, token);
            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpDelete(ApiEndpoints.V1.Movies.DeleteRating)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, token);
            return result ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.V1.Ratings.GetUserRatings)]
        [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserRatings(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, token);
            var ratingsReponse = ratings.MapToResponse();
            return Ok(ratingsReponse);
        }
    }
}
