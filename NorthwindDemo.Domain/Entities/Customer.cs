using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Domain.Entities
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
