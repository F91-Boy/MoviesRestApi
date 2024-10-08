﻿using Microsoft.AspNetCore.OutputCaching;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies
{
    public static class DeleteMovieEndpoint
    {
        public const string Name = "DeleteMovie";

        public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
        {
            app.MapDelete(ApiEndpoints.V1.Movies.Delete, async (
                Guid id, IMovieService movieService, IOutputCacheStore outputCacheStore, CancellationToken token) =>
            {
                var deleted = await movieService.DeleteByIdAsync(id, token);
                if (!deleted)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync("movies", token);//令缓存失效
                return Results.Ok();
            }).WithName(Name);

            return app;
        }
    }
}
