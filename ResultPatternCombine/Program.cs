using System.Text.RegularExpressions;

Result RegisterValidate(string name, int age, string email)
{
    var nameResult = Validator.ValidateRequiredString(name);
    var ageResult = Validator.ValidateAge(age);
    var emailResult = Validator.ValidateEmailWithFormat(email);

    return Result.Combiner(nameResult, ageResult, emailResult);
}

var result = RegisterValidate("Juan", 25, "juan@mail.com");

if (result.IsSuccess)
{
    Console.WriteLine("Registro exitoso");
}
else
{
    Console.WriteLine($"Error en el registro: {result.Error}");
}

public static class Validator
{
    public static Result ValidateRequiredString(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure("Nombre es obligatorio");
        }

        if (name.Length < 2)
        {
            return Result.Failure("Nombre debe tener al menos 2 caracteres");
        }

        return Result.Success();
    }

    public static Result ValidateAge(int age)
    {
        if (age < 0 || age > 120)
        {
            return Result.Failure("Edad debe estar entre 0 y 120");
        }

        return Result.Success();
    }

    public static Result ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            return Result.Failure("Email no es válido");
        }

        return Result.Success();
    }

    public static Result ValidateEmailWithFormat(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$"))
        {
            return Result.Failure("Email no es válido");
        }

        return Result.Success();
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result()
    {
        IsSuccess = true;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Combiner(params Result[] results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                return Failure(result.Error);
            }
        }

        return Success();
    }

    public static Result Success() => new();
    public static Result Failure(string error) => new(error);
}