using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Validators
{
    public class MovieValidator:AbstractValidator<Movie>
    {
        private readonly IMovieRepository _movieRepository;
        public MovieValidator(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;

            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x=>x.Genres).NotEmpty();
            RuleFor(x=>x.Title).NotEmpty();
            RuleFor(x=>x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(x => x.Slug).MustAsync(ValidateSlug).WithMessage("该电影已经存在于系统中");
        }

        private async Task<bool> ValidateSlug(Movie movie,string slug ,CancellationToken token)
        {
            var existingMovie = await _movieRepository.GetBySlugAsync(slug);

            if (existingMovie is not null) 
            {
                return existingMovie.Id == movie.Id;//false
            }

            return existingMovie is null;//true
        }
    }
}
