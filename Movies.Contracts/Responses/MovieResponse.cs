using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {

        public required Guid Id { get; init; }
        
        public required string Title { get; init; }

        public required int YearOfRelease { get; init; }

        public required IEnumerable<string> Genders { get; init; } = Enumerable.Empty<string>();
    }
}
