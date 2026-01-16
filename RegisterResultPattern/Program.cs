using System.Text.RegularExpressions;

var userService = new UserService();

Console.WriteLine("Escribe un correo: ");
var email = Console.ReadLine();

Console.WriteLine("Escribe una contraseña: ");
var password = Console.ReadLine();

Console.WriteLine("Confirma la contraseña: ");
var confirmPassword = Console.ReadLine();

var resgisterResult = userService.Register(email, password, confirmPassword);

if (resgisterResult.IsSuccess)
{
    Console.WriteLine($"Usuario registrado con éxito. Id: {resgisterResult.Value!.Id}.");
}
else
{
    Console.WriteLine(resgisterResult.Error);
}

public class  UserService
{
    private readonly List<User> _users = new();

    public Result<User> Register(string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Result<User>.Failure("Email y contraseña obligatorios");
        }

        if (!Regex.IsMatch(email, @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$"))
        {
            return Result<User>.Failure($"El email [{email}] es inválido");
        }

        if (_users.Any(u => u.Email.Equals(email)))
        {
            return Result<User>.Failure($"El email [{email}] ya está registrado");
        }

        if (password != confirmPassword)
        {
            return Result<User>.Failure("Las contraseñas no coinciden");
        }

        var user = new User(Guid.NewGuid(), email, password);
        _users.Add(user);

        return Result<User>.Success(user);
    }
}

public record User(Guid Id, string Email, string Password);

public class Result<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);
}