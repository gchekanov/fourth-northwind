using FluentValidation;
using NorthwindDemo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.Validators
{
    public class CustomerQueryValidator : AbstractValidator<CustomerQuery>
    {
        private static readonly string[] AllowedSortColumns = { "name", "companyName", "city", "country" };

        public CustomerQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be 1 or greater.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("PageSize must be between 1 and 50.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => AllowedSortColumns.Contains(sortBy))
                .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortColumns)}");

            RuleFor(x => x.Search)
                .MaximumLength(50)
                .WithMessage("Search term must be 50 characters or less.");
        }
    }
}
