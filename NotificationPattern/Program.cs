var customer = new Customer("", "", -15);

var notification = CustomerValidation.Validate(customer);

if (notification.HasErrors)
{
    notification.Errors.ToList().ForEach(error => Console.WriteLine($" - {error}"));
}
else
{
    Console.WriteLine("Cliente valido");
}

public record Customer(string Name, string Country, decimal Balance);

public static class CustomerValidation
{
    public static Notification Validate(Customer customer)
    {
        var notification = new Notification();

        if (string.IsNullOrWhiteSpace(customer.Name))
        {
            notification.Add("Nombre es obligatorio");
        }
        if (string.IsNullOrWhiteSpace(customer.Country))
        {
            notification.Add("País es obligatorio");
        }
        if (customer.Balance < 0)
        {
            notification.Add("Saldo no puede ser negativo");
        }

        return notification;
    }
}

public class Notification
{
    private readonly List<string> _errors = new();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();
    public bool HasErrors => _errors.Any();
    public void Add(string error)
    {
        _errors.Add(error);
    }
}