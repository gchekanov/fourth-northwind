using Microsoft.EntityFrameworkCore;
using NorthwindDemo.Application.Services;
using NorthwindDemo.Domain.Entities;
using NorthwindDemo.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Api.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private NorthwindDbContext _context;
        private CustomerService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NorthwindDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new NorthwindDbContext(options);

            SeedDatabase(_context);

            _service = new CustomerService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private void SeedDatabase(NorthwindDbContext context)
        {
            var product1 = new Product { ProductId = 1, ProductName = "Prod1", UnitsInStock = 10, Discontinued = false };
            var product2 = new Product { ProductId = 2, ProductName = "Prod2", UnitsInStock = 0, Discontinued = true };

            context.Products.AddRange(product1, product2);

            var customer1 = new Customer { CustomerId = "C1", CompanyName = "Alpha" };
            var customer2 = new Customer { CustomerId = "C2", CompanyName = "Beta" };

            context.Customers.AddRange(customer1, customer2);

            var order1 = new Order { OrderId = 1, CustomerId = "C1" };
            var order2 = new Order { OrderId = 2, CustomerId = "C1" };

            context.Orders.AddRange(order1, order2);

            var od1 = new OrderDetail {  OrderId = 1, ProductId = 1, UnitPrice = 100, Quantity = 2 };
            var od2 = new OrderDetail {  OrderId = 1, ProductId = 2, UnitPrice = 50, Quantity = 1 };
            var od3 = new OrderDetail {  OrderId = 2, ProductId = 1, UnitPrice = 100, Quantity = 5 };

            context.OrderDetails.AddRange(od1, od2, od3);

            context.SaveChanges();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCustomerWithOrders()
        {
            // Act
            var result = await _service.GetByIdAsync("C1", CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("C1", result.Id);
            Assert.AreEqual("Alpha", result.Name);
            Assert.AreEqual(2, result.Orders.Count);

            var firstOrder = result.Orders.First();
            Assert.AreEqual(2, firstOrder.ProductCount);
            Assert.IsTrue(firstOrder.HasIssues);
            Assert.Contains("Discontinued product", firstOrder.Issues);
            Assert.Contains("Stock issue", firstOrder.Issues);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            var result = await _service.GetByIdAsync("NonExisting", CancellationToken.None);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnPagedCustomers()
        {
            var query = new Application.Models.CustomerQuery
            {
                Page = 1,
                PageSize = 1,
                SortBy = "name",
                Desc = false
            };

            var result = await _service.GetAllAsync(query, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual("Alpha", result.Items.First().Name);
        }

        [Test]
        public async Task GetAllAsync_ShouldFilterBySearch()
        {
            var query = new Application.Models.CustomerQuery { Search = "Beta" };
            var result = await _service.GetAllAsync(query, CancellationToken.None);

            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual("Beta", result.Items.First().Name);
        }

        [Test]
        public async Task GetCustomerOrdersAsync_ShouldReturnOrdersWithIssues()
        {
            var result = await _service.GetCustomerOrdersAsync("C1", CancellationToken.None);

            Assert.AreEqual(2, result.Count);

            var firstOrder = result.First();
            Assert.AreEqual(2, firstOrder.ProductCount);
            Assert.IsTrue(firstOrder.HasIssues);
            Assert.Contains("Discontinued product", firstOrder.Issues);
            Assert.Contains("Stock issue", firstOrder.Issues);
        }

        [Test]
        public async Task GetCustomerOrdersAsync_ShouldReturnEmptyList_WhenCustomerHasNoOrders()
        {
            var result = await _service.GetCustomerOrdersAsync("C2", CancellationToken.None);
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
