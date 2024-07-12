using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = [];

        //创建
        public Task<bool> CreateAsync(Movie movie)
        {
            _movies.Add(movie);
            return Task.FromResult(true);
        }

        //删除
        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var removedCount = _movies.RemoveAll(x => x.Id == id);
            var movieRemoved = removedCount > 0;
            return Task.FromResult(movieRemoved);
        }

        //获取所有
        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            return Task.FromResult(_movies.AsEnumerable());
        }

        //获取单个
        public Task<Movie?> GetByIdAsync(Guid id)
        {
           var movie = _movies.SingleOrDefault(x => x.Id == id);
            return Task.FromResult(movie);
        }

        //更新
        public Task<bool> UpdateAsync(Movie movie)
        {
           var movieIndex = _movies.FindIndex(x=>x.Id == movie.Id);
            if (movieIndex == -1) 
            {
                return Task.FromResult(false);   
            }

            _movies[movieIndex] = movie;
            return Task.FromResult(true);
        }
    }
}
