namespace ToolsBazaar.Domain.CustomerAggregate;

public interface ICustomerRepository
{
    void UpdateCustomerName(int customerId, string name);
    IEnumerable<Customer> GetAll();
}