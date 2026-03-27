using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindDemo.Application.Tests
{
    [TestFixture]
    public class CustomersIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task Customers_Index_ReturnsSuccessAndContainsTable()
        {
            var response = await _client.GetAsync("/Customers");
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(html.Contains("<table"));
            Assert.IsTrue(html.Contains("Customers"));
        }
    }
}
