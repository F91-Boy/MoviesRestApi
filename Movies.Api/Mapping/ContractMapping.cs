using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping
{
    public static class ContractMapping
    {
        /// <summary>
        /// 创建请求体 map 成 电影
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Movie MapToMovie(this CreateMovieRequest request)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList()
            };
        }

        /// <summary>
        /// 更新请求体 map 成 电影
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id = id,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList()
            };
        }

        /// <summary>
        /// 电影 map 成 响应体
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static MovieResponse MapToResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
                Slug = movie.Slug,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
                Genres = movie.Genres
            };

        }

        /// <summary>
        /// 电影集合 map 成 集合响应体
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
        {
            return new MoviesResponse
            {
                Items = movies.Select(MapToResponse)
            };

        }

        /// <summary>
        ///  带电影的评分集合 map 成 带电影的评分集合的响应体
        /// </summary>
        /// <param name="ratings"></param>
        /// <returns></returns>
        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
        {
            return ratings.Select(x => new MovieRatingResponse
            {
                MovieId = x.MovieId,
                Rating = x.Rating,
                Slug = x.Slug
            });
        }


        public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
        {
            return new GetAllMoviesOptions
            {
                Title = request.Title,
                YearOfRelease = request.Year,
                SortField = request.SortBy?.Trim('+', '-'),
                SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                           (request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending),
            };
        }


        public static GetAllMoviesOptions WithUserId(this GetAllMoviesOptions options, Guid? userId)
        {
            options.UserId = userId;
            return options;
        }

    }
}
