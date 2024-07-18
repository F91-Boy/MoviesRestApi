using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings
{
    public static class GetUserRatingEndpoint
    {
        public const string Name = "GetUserRating";

        public static IEndpointRouteBuilder MapGetUserRating(this IEndpointRouteBuilder app)
        {
            app.MapGet(ApiEndpoints.V1.Ratings.GetUserRatings, async (
                IRatingService ratingService, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();
                var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
                var ratingsReponse = ratings.MapToResponse();
                return TypedResults.Ok(ratingsReponse);
            }).WithName(Name);

            return app;
        }
    }
}
