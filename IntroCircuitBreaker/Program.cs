using Polly;
using Polly.CircuitBreaker;

var circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                            .CircuitBreakerAsync(
                                exceptionsAllowedBeforeBreaking: 3,
                                durationOfBreak: TimeSpan.FromSeconds(5),
                                onBreak: (exception, breakDelay) =>
                                {
                                    Console.WriteLine($"Circuito abierto: {exception.Message}. No se intentará nuevamente por {breakDelay.TotalSeconds} segundos");
                                },
                                onReset: () =>
                                {
                                    Console.WriteLine($"Circuito cerrado: el servicio volvió a funcionar.");
                                },
                                onHalfOpen: () =>
                                {
                                    Console.WriteLine($"Circuito medio abierto: probando nuevamente.");
                                }
                             );

using var httpClient = new HttpClient();

for (int i = 1; i <= 10; i++)
{
    Console.WriteLine($"Intento {i}");

    try
    {
        await circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            var response = await httpClient.GetAsync("https://metalcode2.io");
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Solicitud exitosa.");
        });
    }
    catch(BrokenCircuitException)
    {
        Console.WriteLine("El circuito está abierto, no se realizará la llamada.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    await Task.Delay(2000);
}