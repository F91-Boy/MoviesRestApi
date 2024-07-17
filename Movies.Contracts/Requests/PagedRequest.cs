using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Requests
{
    public class PagedRequest
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 10;

        //public required int Page { get; init; } 

        //public required int PageSize { get; init; } 

        public int?  Page { get; init; } = DefaultPage;

        public int?  PageSize { get; init; } = DefaultPageSize;
    }
}
