using NorthwindDemo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.Queries
{
    public class GetCustomersQuery
    {
        public CustomerQuery Filter { get; set; }
    }
}
