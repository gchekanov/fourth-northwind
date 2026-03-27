using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.Models
{
    public class CustomerQuery
    {
        public string? Search { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "name";
        public bool Desc { get; set; } = false;
    }
}
