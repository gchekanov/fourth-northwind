using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NorthwindDemo.API.Helpers;
using NorthwindDemo.Application.Interfaces;
using NorthwindDemo.Application.Models;

namespace NorthwindDemo.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly IValidator<CustomerQuery> _validator;

        public CustomersController(
            ICustomerService service,
            IValidator<CustomerQuery> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet("customers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> GetAll([FromQuery] CustomerQuery query, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

            var result = await _service.GetAllAsync(query, ct);
            var etag = ETagHelper.Generate(result.Items, query.Page, query.PageSize);

            if (Request.Headers.TryGetValue("If-None-Match", out var incomingEtag) && incomingEtag == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            Response.Headers["ETag"] = etag;

            return Ok(new
            {
                items = result.Items,
                totalCount = result.TotalCount,
                page = query.Page,
                pageSize = query.PageSize
            });
        }

        [HttpGet("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> Get(string id, CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);

            if (result == null)
                return NotFound();
            var etag = ETagHelper.Generate(result);
            Response.Headers["ETag"] = etag;
            if (Request.Headers.TryGetValue("If-None-Match", out var incomingEtag) && incomingEtag == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }
            return Ok(result);
        }

        [HttpGet("{id}/orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<IActionResult> GetCustomerOrders(string id, CancellationToken ct)
        {
            var orders = await _service.GetCustomerOrdersAsync(id, ct);

            if (orders == null || !orders.Any())
                return NotFound();
            var etag = ETagHelper.Generate(orders);
            Response.Headers["ETag"] = etag;
            if (Request.Headers.TryGetValue("If-None-Match", out var incomingEtag) && incomingEtag == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }
            return Ok(orders);
        }
    }
}
