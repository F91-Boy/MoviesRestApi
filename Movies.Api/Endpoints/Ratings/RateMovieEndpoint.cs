using Azure.Core;
using Movies.Api.Auth;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Ratings
{
    public static class RateMovieEndpoint
    {
        public const string Name = "RateMovie";

        public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.V1.Movies.Rate, async (
                IRatingService ratingService, Guid id,
                RateMovieRequest request, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var result = await ratingService.RateMovieAsync(id, request.Rating.GetValueOrDefault(5), userId!.Value, token);
                return result ? Results.Ok() : Results.NotFound();
            }).WithName(Name);

            return app;
        }
    }
}
