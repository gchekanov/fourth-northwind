using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.DTOs
{
    public class CustomerDetailsDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<OrderDto> Orders { get; set; }

    }
}
