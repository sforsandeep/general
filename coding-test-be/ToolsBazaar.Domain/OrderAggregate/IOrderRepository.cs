namespace ToolsBazaar.Domain.OrderAggregate;

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
}