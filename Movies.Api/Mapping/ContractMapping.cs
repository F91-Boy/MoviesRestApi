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
                Genders = request.Genders.ToList()
            };
        }

        /// <summary>
        /// 更新请求体 map 成 电影
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Movie MapToMovie(this UpdateMovieRequest request,Guid id)
        {
            return new Movie
            {
                Id = id,    
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genders = request.Genders.ToList()
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
                Genders = movie.Genders
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
    }
}
