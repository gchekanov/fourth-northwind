using NorthwindDemo.Application.DTOs;
using NorthwindDemo.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedResult<CustomerListDto>> GetAllAsync(CustomerQuery search, CancellationToken ct);
        Task<CustomerDetailsDto> GetByIdAsync(string id, CancellationToken ct);
        Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId, CancellationToken ct);
    }
}
