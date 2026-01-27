var retryOperation = new Retry<string>(async () =>
{
    var httpClient = new HttpClient();
    var response = await httpClient.GetAsync("https://example.com/api/data");

    if (!response.IsSuccessStatusCode)
    {
        throw new Exception($"Operación fallida con código de estado {response.StatusCode}");
    }

    return await response.Content.ReadAsStringAsync();
}, 5, 2000);

try
{
    string result = await retryOperation.Execute();
    Console.WriteLine($"Operación exitosa con resultado: {result.Substring(0, 50)}...");
}
catch (Exception ex)
{
    Console.WriteLine($"La operación ha fallado después de varios intentos: {ex.Message}");
}

public class Retry<T>
{
    private readonly Func<Task<T>> _operation;
    private readonly int _maxRetries;
    private readonly int _delayMiliseconds;

    public Retry(Func<Task<T>> operation, int maxRetries = 3, int delayMiliseconds = 1000)
    {
        _operation = operation;
        _maxRetries = maxRetries;
        _delayMiliseconds = delayMiliseconds;
    }

    public async Task<T> Execute()
    {
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return await _operation();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Intento {attempt} ha fallado: {ex.Message}");
                await Task.Delay(_delayMiliseconds);
            }
        }

        throw new Exception("Todos los reintentos han fallado.");
    }
}