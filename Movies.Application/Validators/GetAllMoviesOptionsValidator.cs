﻿using FluentValidation;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Validators
{
    public class GetAllMoviesOptionsValidator:AbstractValidator<GetAllMoviesOptions>
    {
        private readonly string[] AcceptableSortFields = ["yearofrelease","title"];


        public GetAllMoviesOptionsValidator()
        {
            RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);

            RuleFor(x => x.SortField)
                .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("排序的字段只能是'title'或者'yearofrelease'");

            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

            RuleFor(x=>x.PageSize).InclusiveBetween(1, 25)
                .WithMessage("每页最多获取1到25条电影信息");
        }
    }
}
