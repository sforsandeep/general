namespace ToolsBazaar.Domain.CustomerAggregate;

public class Customer
{
    private string _name;
    public int Id { get; init; }

    public string Name
    {
        get => _name;
        init => _name = value;
    }

    public string Email { get; init; }
    public string Address { get; init; }

    public void UpdateName(string name) => _name = name;
}