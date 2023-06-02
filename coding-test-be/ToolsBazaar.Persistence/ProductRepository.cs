using ToolsBazaar.Domain.ProductAggregate;

namespace ToolsBazaar.Persistence;

public class ProductRepository : IProductRepository
{
    public IEnumerable<Product> GetAll() => DataSet.AllProducts;
}