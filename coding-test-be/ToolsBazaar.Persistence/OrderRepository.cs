using ToolsBazaar.Domain.OrderAggregate;

namespace ToolsBazaar.Persistence;

public class OrderRepository : IOrderRepository
{
    public IEnumerable<Order> GetAll() => DataSet.AllOrders;
}