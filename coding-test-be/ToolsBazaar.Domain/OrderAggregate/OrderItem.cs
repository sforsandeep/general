using ToolsBazaar.Domain.ProductAggregate;

namespace ToolsBazaar.Domain.OrderAggregate;

public class OrderItem
{
    public int Id { get; init; }
    public int Quantity { get; init; }
    public Product Product { get; init; }
    public decimal Price => Quantity * Product.Price;
}