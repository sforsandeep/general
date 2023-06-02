namespace ToolsBazaar.Domain.OrderAggregate;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
    IEnumerable<Order> GetOrdersInDateRange(DateTime startDate, DateTime endDate);
}