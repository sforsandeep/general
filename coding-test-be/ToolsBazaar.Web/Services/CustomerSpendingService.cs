using ToolsBazaar.Domain.CustomerAggregate;
using ToolsBazaar.Domain.OrderAggregate;

namespace ToolsBazaar.Web.Services
{
    public class CustomerSpendingService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CustomerSpendingService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public IEnumerable<CustomerSpending> GetTopCustomersBySpending(DateTime startDate, DateTime endDate, int top)
        {
            var orders = _orderRepository.GetOrdersInDateRange(startDate, endDate);

            var customerSpendings = _customerRepository.GetAll()
                .Select(c => new CustomerSpending
                {
                    Customer = c,
                    TotalSpending = orders
                        .Where(o => o.Customer.Id == c.Id)
                        .Sum(o => o.Items.Sum(i => i.Quantity * i.Product.Price))
                })
                .OrderByDescending(cs => cs.TotalSpending)
                .Take(top);

            return customerSpendings;
        }
    }

    public class CustomerSpending
    {
        public Customer Customer { get; set; }
        public decimal TotalSpending { get; set; }
    }

}
