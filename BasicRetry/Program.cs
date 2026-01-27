var retryOperation = new Retry<int>(() =>
{
    Random rand = new Random();
    int value = rand.Next(0, 10);
    if (value < 7)
    {
        throw new Exception($"Operación fallida, número erróneo {value}");
    }
    return value;
}, 4);

try
{
    int result = retryOperation.Execute();
    Console.WriteLine($"Operación exitosa con resultado: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Operación fallida después de varios intentos: {ex.Message}");
}

public class Retry<T>
{
    private readonly Func<T> _operation;
    private readonly int _maxRetries;

    public Retry(Func<T> operation, int maxRetries = 3)
    {
        _operation = operation;
        _maxRetries = maxRetries;
    }

    public T Execute()
    {
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return _operation();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Intento {attempt} ha fallado: {ex.Message}");
            }
        }

        throw new Exception("Todos los reintentos han fallado.");
    }
}