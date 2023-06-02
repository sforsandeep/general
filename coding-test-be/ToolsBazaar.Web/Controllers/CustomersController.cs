using Microsoft.AspNetCore.Mvc;
using ToolsBazaar.Domain.CustomerAggregate;
using ToolsBazaar.Web.Services;

namespace ToolsBazaar.Web.Controllers;

public record CustomerDto(string Name);

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomersController> _logger;
    private readonly CustomerSpendingService _customerSpendingService;
    public CustomersController(ILogger<CustomersController> logger, ICustomerRepository customerRepository, CustomerSpendingService customerSpendingService)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _customerSpendingService = customerSpendingService;
    }

    [HttpPut("{customerId:int}")]
    public IActionResult UpdateCustomerName(int customerId, [FromBody] CustomerDto dto)
    {
        _logger.LogInformation($"Updating customer #{customerId} name...");

        try
        {
            _customerRepository.UpdateCustomerName(customerId, dto.Name);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error updating customer name");
            return NotFound(ex.Message);
        }
    }
    [HttpGet("top")]
    public IActionResult GetTopCustomersBySpending()
    {
        var startDate = new DateTime(2015, 1, 1);
        var endDate = new DateTime(2022, 12, 31);
        var top = 5;


        var topCustomers = _customerSpendingService.GetTopCustomersBySpending(startDate, endDate, top);

        return Ok(topCustomers);
    }
}