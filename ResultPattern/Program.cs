var readNumer = new ReadNumer();
var numberResult = readNumer.Read();

if (numberResult.IsSuccess)
{
    Console.WriteLine($"Número leído: {numberResult.Value}");
}
else
{
    Console.WriteLine($"Error: {numberResult.Error}");
}

public class ReadNumer
{
    public Result<int> Read()
    {
        Console.Write("Escribe un número: ");
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int number))
        {
            var result = Result<int>.Failure($"El valor [{input}] no es un número válido");
            return result;
        }

        return Result<int>.Success(number);
    }
}

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