using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Movies
{
    public static class UpdateMovieEndpotion
    {
        public const string Name = "UpdateMovie";

        public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
        {
            app.MapPut(ApiEndpoints.V1.Movies.Update, async (
                Guid id,UpdateMovieRequest  request,IOutputCacheStore outputCacheStore,
                IMovieService movieService, HttpContext context, CancellationToken token) =>
            {
                var userId = context.GetUserId();

                var movie = request.MapToMovie(id);
                var updatedMovie = await movieService.UpdateAsync(movie, userId, token);
                if (updatedMovie is null)
                {
                    return Results.NotFound();
                }

                var response = movie.MapToResponse();

                await outputCacheStore.EvictByTagAsync("movies", token);//令缓存失效
                return TypedResults.Ok(response);
            }).WithName(Name);

            return app;
        }
    }
}
