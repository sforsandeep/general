namespace ToolsBazaar.Domain.ProductAggregate;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();
}