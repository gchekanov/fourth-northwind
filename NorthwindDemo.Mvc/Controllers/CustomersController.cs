using Microsoft.AspNetCore.Mvc;
using NorthwindDemo.Mvc.Models;
using System.Net.Http.Json;

namespace NorthwindDemo.Mvc.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _http;

        public CustomersController(IHttpClientFactory httpFactory)
        {
            _http = httpFactory.CreateClient("NorthwindApi");
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var pageSize = 10;
            var url = $"customers?search={search ?? ""}&page={page}&pageSize={pageSize}";

            var response = await _http.GetFromJsonAsync<PagedResult<CustomerViewModel>>(url);

            if (response == null)
            {
                response = new PagedResult<CustomerViewModel>
                {
                    Items = new List<CustomerViewModel>(),
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0
                };
            }

            ViewData["Search"] = search;

            return View(response);
        }

        public async Task<IActionResult> Details(string id)
        {
            var customer = await _http.GetFromJsonAsync<CustomerDetailsViewModel>(
                $"customer/{id}"
            );

            if (customer == null)
                return NotFound();

            return View(customer);
        }
    }
}