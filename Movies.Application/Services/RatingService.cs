using FluentValidation.Results;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
             _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;
        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
           return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
        }

        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
        {
            if (rating is <=0 or >5)
            {
                throw new FluentValidation.ValidationException(new[]
                { 
                    new ValidationFailure
                    {
                        PropertyName = "Rating",
                        ErrorMessage = "评分必须介于1到5之间"
                    }
                });
             
            }

            var movieExists = await _movieRepository.ExistsByIdAsync(movieId,token);
            if (!movieExists) 
            {
                return false;
            }

            return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
        }
    }
}
