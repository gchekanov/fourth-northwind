using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int ProductCount { get; set; }

        public bool HasIssues { get; set; }
        public List<string> Issues { get; set; } = new();

    }
}
