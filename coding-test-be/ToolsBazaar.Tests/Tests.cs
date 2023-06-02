using FluentAssertions;
using NSubstitute;
using ToolsBazaar.Domain.CustomerAggregate;
using ToolsBazaar.Domain.OrderAggregate;
using ToolsBazaar.Domain.ProductAggregate;
using ToolsBazaar.Web.Services;

namespace ToolsBazaar.Tests;

public class Tests
{
    [Fact]
    public void SampleTest()
    {
        var x = 10;

        x.Should().Be(10);
    }

    [Fact]
    public void GetTopCustomersBySpending_ReturnsCorrectCustomers()
    {
        // Arrange
        var startDate = new DateTime(2015, 1, 1);
        var endDate = new DateTime(2022, 12, 31);
        var top = 5;

        var customerRepository = Substitute.For<ICustomerRepository>();
        var orderRepository = Substitute.For<IOrderRepository>();

        // mock data.
        var mockCustomers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Customer 1" },
            new Customer { Id = 3, Name = "Customer 3" },
            new Customer { Id = 5, Name = "Customer 5" },
            new Customer { Id = 7, Name = "Customer 7" },
            new Customer { Id = 9, Name = "Customer 9" },
            new Customer { Id = 11, Name = "Customer 11" },
            new Customer { Id = 13, Name = "Customer 13" },
        };

        var mockOrders = new List<Order>
        {
            new Order { Id = 1, Customer = mockCustomers[0], Date = new DateTime(2016, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 2, Product = new Product { Price = 50m } } } },
            new Order { Id = 2, Customer = mockCustomers[1], Date = new DateTime(2017, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 3, Product = new Product { Price = 60m } } } },
            new Order { Id = 3, Customer = mockCustomers[2], Date = new DateTime(2018, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 4, Product = new Product { Price = 70m } } } },
            new Order { Id = 4, Customer = mockCustomers[3], Date = new DateTime(2019, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 5, Product = new Product { Price = 80m } } } },
            new Order { Id = 5, Customer = mockCustomers[4], Date = new DateTime(2020, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 6, Product = new Product { Price = 90m } } } },
            new Order { Id = 6, Customer = mockCustomers[5], Date = new DateTime(2021, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 7, Product = new Product { Price = 100m } } } },
            new Order { Id = 7, Customer = mockCustomers[6], Date = new DateTime(2022, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 8, Product = new Product { Price = 110m } } } },
        };

        customerRepository.GetAll().Returns(mockCustomers);
        orderRepository.GetOrdersInDateRange(startDate, endDate).Returns(mockOrders);

        var service = new CustomerSpendingService(customerRepository, orderRepository);

        // Act
        var result = service.GetTopCustomersBySpending(startDate, endDate, top);

        // Assert
        result.Should().HaveCount(top);

        // Check if the customers are sorted by total spending in descending order
        result.Should().BeInDescendingOrder(cs => cs.TotalSpending);

        // Check if the top spending customer is the one with Id = 13
        result.First().Customer.Id.Should().Be(13);
    }

    [Fact]
    public void GetTopCustomersBySpending_ExcludesCustomersWithNoOrders()
    {
        // Arrange
        var startDate = new DateTime(2015, 1, 1);
        var endDate = new DateTime(2022, 12, 31);
        var top = 5;

        var customerRepository = Substitute.For<ICustomerRepository>();
        var orderRepository = Substitute.For<IOrderRepository>();

        // Set up mock data
        var mockCustomers = new List<Customer>
    {
        new Customer { Id = 1, Name = "Customer 1" },
        new Customer { Id = 3, Name = "Customer 3" },
        new Customer { Id = 5, Name = "Customer 5" },
        new Customer { Id = 7, Name = "Customer 7" },
        new Customer { Id = 9, Name = "Customer 9" },
        new Customer { Id = 11, Name = "Customer 11" },
        new Customer { Id = 13, Name = "Customer 13" },
    };

        var mockOrders = new List<Order>
    {
        new Order { Id = 1, Customer = mockCustomers[0], Date = new DateTime(2016, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 2, Product = new Product { Price = 50m } } } },
        new Order { Id = 2, Customer = mockCustomers[1], Date = new DateTime(2017, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 3, Product = new Product { Price = 60m } } } },
        // Customer 5 has no orders
        new Order { Id = 4, Customer = mockCustomers[3], Date = new DateTime(2019, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 5, Product = new Product { Price = 80m } } } },
        new Order { Id = 5, Customer = mockCustomers[4], Date = new DateTime(2020, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 6, Product = new Product { Price = 90m } } } },
        new Order { Id = 6, Customer = mockCustomers[5], Date = new DateTime(2021, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 7, Product = new Product { Price = 100m } } } },
        new Order { Id = 7, Customer = mockCustomers[6], Date = new DateTime(2022, 1, 1), Items = new List<OrderItem> { new OrderItem { Quantity = 8, Product = new Product { Price = 110m } } } },
    };

        customerRepository.GetAll().Returns(mockCustomers);
        orderRepository.GetOrdersInDateRange(startDate, endDate).Returns(mockOrders);

        var service = new CustomerSpendingService(customerRepository, orderRepository);

        // Act
        var result = service.GetTopCustomersBySpending(startDate, endDate, top);

        // Assert
        result.Should().NotContain(cs => cs.Customer.Id == 5);
    }

}