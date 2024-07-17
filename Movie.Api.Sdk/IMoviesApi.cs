using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk
{
    [Headers("Authorization:Bearer")]
    public interface IMoviesApi
    {
        [Get(ApiEndpoints.V1.Movies.Get)]
        Task<MovieResponse> GetMovieAsync(string idOrSlug);

        [Get(ApiEndpoints.V1.Movies.GetAll)]
        Task<MoviesResponse> GetAllMoviesAsync(GetAllMoviesRequest request);

        [Post(ApiEndpoints.V1.Movies.Create)]
        Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);

        [Put(ApiEndpoints.V1.Movies.Update)]
        Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);

        [Delete(ApiEndpoints.V1.Movies.Delete)]
        Task DeleteMovieAsync(Guid id);

        [Put(ApiEndpoints.V1.Movies.Rate)]
        Task<MovieResponse> RateMovieAsync(Guid id, RateMovieRequest request);


        [Delete(ApiEndpoints.V1.Movies.DeleteRating)]
        Task DeleteRatingAsync(Guid id);

        [Get(ApiEndpoints.V1.Ratings.GetUserRatings)]
        Task<IEnumerable<MovieRatingResponse>> GetUserRatingAsync();






    }
}
