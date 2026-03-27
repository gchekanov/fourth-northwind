using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NorthwindDemo.Mvc.Controllers;
using NorthwindDemo.Mvc.Models;
using System.Net;
using System.Net.Http.Json;

[TestFixture]
public class CustomersControllerTests
{
    private CustomersController _controller;
    private Mock<IHttpClientFactory> _httpFactoryMock;

    [SetUp]
    public void Setup()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
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
               });

        var httpClient = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("https://fakeapi.local/")
        };

        _httpFactoryMock = new Mock<IHttpClientFactory>();
        _httpFactoryMock.Setup(f => f.CreateClient("NorthwindApi")).Returns(httpClient);

        _controller = new CustomersController(_httpFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _controller = null;
        _httpFactoryMock = null;
    }

    [Test]
    public async Task Index_ReturnsViewWithCustomers()
    {
        var result = await _controller.Index(null);
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as PagedResult<CustomerViewModel>;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Items.Count);
        Assert.AreEqual("Alpha", model.Items.First().Name);
    }
}