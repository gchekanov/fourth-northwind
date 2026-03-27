using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NorthwindDemo.Application.DTOs;
using NorthwindDemo.Application.Interfaces;
using NorthwindDemo.Application.Models;
using NorthwindDemo.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NorthwindDemo.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly NorthwindDbContext _context;

        public CustomerService(
            NorthwindDbContext context)
        {
            _context = context;
        }


        public async Task<CustomerDetailsDto> GetByIdAsync(string id, CancellationToken ct)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == id, ct);

            if (customer == null)
            {
                return null;
            }

            var orders = customer.Orders.Select(o =>
            {
                var dto = new OrderDto
                {
                    Id = o.OrderId,
                    ProductCount = o.OrderDetails.Count,
                    TotalAmount = o.OrderDetails.Sum(d => d.UnitPrice * d.Quantity)
                };

                foreach (var d in o.OrderDetails)
                {
                    if (d.Product.Discontinued)
                        dto.Issues.Add("Discontinued product");

                    if (d.Product.UnitsInStock < d.Quantity)
                        dto.Issues.Add("Stock issue");
                }

                dto.HasIssues = dto.Issues.Any();
                return dto;
            }).ToList();

            return new CustomerDetailsDto
            {
                Id = customer.CustomerId,
                Name = customer.CompanyName,
                Orders = orders
            };
        }


        public async Task<PagedResult<CustomerListDto>> GetAllAsync(CustomerQuery query, CancellationToken ct)
        {
            var customers = _context.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                customers = customers.Where(c => c.CompanyName.Contains(search));
            }

            var projected = customers.Select(c => new CustomerListDto
            {
                Id = c.CustomerId,
                Name = c.CompanyName,
                OrderCount = c.Orders.Count()
            });

            var total = await projected.CountAsync(ct);

            projected = query.SortBy?.ToLower() switch
            {
                "orders" => query.Desc
                    ? projected.OrderByDescending(x => x.OrderCount)
                    : projected.OrderBy(x => x.OrderCount),
                _ => query.Desc
                    ? projected.OrderByDescending(x => x.Name)
                    : projected.OrderBy(x => x.Name)
            };

            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 :
                           query.PageSize > 50 ? 50 : query.PageSize;

            var items = await projected
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<CustomerListDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(string customerId, CancellationToken ct)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsNoTracking()
                .ToListAsync(ct);

            var result = orders.Select(o =>
            {
                var dto = new OrderDto
                {
                    Id = o.OrderId,
                    ProductCount = o.OrderDetails.Count,
                    TotalAmount = o.OrderDetails.Sum(d => d.UnitPrice * d.Quantity)
                };

                foreach (var d in o.OrderDetails)
                {
                    if (d.Product.Discontinued)
                        dto.Issues.Add("Discontinued product");

                    if (d.Product.UnitsInStock < d.Quantity)
                        dto.Issues.Add("Stock issue");
                }

                dto.HasIssues = dto.Issues.Any();
                return dto;
            }).ToList();

            return result;
        }
    }
}
