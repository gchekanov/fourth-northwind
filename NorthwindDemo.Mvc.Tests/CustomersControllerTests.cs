using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NorthwindDemo.Mvc.Controllers;
using NorthwindDemo.Mvc.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

namespace NorthwindDemo.Mvc.Tests
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomersController _controller;
        private Mock<IHttpClientFactory> _httpFactoryMock;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                       .Setup<Task<HttpResponseMessage>>(
                           "SendAsync",
                           ItExpr.IsAny<HttpRequestMessage>(),
                           ItExpr.IsAny<CancellationToken>()
                       )
                       .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                       {
                           if (request.RequestUri!.AbsolutePath.Contains("customers"))
                           {
                               return new HttpResponseMessage(HttpStatusCode.OK)
                               {
                                   Content = JsonContent.Create(new PagedResult<CustomerViewModel>
                                   {
                                       Items = new List<CustomerViewModel>
                                       {
                                           new CustomerViewModel { Id = "C1", Name = "Alpha", OrderCount = 2 }
                                       },
                                       TotalCount = 1,
                                       Page = 1,
                                       PageSize = 10
                                   })
                               };
                           }

                           if (request.RequestUri!.AbsolutePath.Contains("customer/C1"))
                           {
                               return new HttpResponseMessage(HttpStatusCode.OK)
                               {
                                   Content = JsonContent.Create(new CustomerDetailsViewModel
                                   {
                                       Id = "C1",
                                       Name = "Alpha",
                                       Orders = new List<OrderViewModel>
                                       {
                                           new OrderViewModel { Id = 1, ProductCount = 2, TotalAmount = 200 }
                                       }
                                   })
                               };
                           }

                           return new HttpResponseMessage(HttpStatusCode.NotFound);
                       });

            _httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://fakeapi.local/")
            };

            _httpFactoryMock = new Mock<IHttpClientFactory>();
            _httpFactoryMock.Setup(f => f.CreateClient("NorthwindApi")).Returns(_httpClient);

            _controller = new CustomersController(_httpFactoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
            _httpFactoryMock = null!;
            _httpClient.Dispose();
        }

        [Test]
        public async Task Index_ShouldReturnViewWithPagedCustomers()
        {
            var result = await _controller.Index(null) as ViewResult;

            Assert.IsNotNull(result);

            var model = result!.Model as PagedResult<CustomerViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model!.Items.Count);
            Assert.AreEqual("Alpha", model.Items.First().Name);
        }

        [Test]
        public async Task Details_ShouldReturnViewWithCustomerDetails()
        {
            var result = await _controller.Details("C1") as ViewResult;

            Assert.IsNotNull(result);

            var model = result!.Model as CustomerDetailsViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("C1", model!.Id);
            Assert.AreEqual("Alpha", model.Name);
            Assert.AreEqual(1, model.Orders.Count);
        }
    }

}