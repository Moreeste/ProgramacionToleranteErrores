using Polly;

var fallbackPolicy = Policy<decimal>.Handle<HttpRequestException>()
                        .FallbackAsync(
                            fallbackValue: 18.03m,
                            onFallbackAsync: async (exception, context) =>
                            {
                                Console.WriteLine("Fallback: servicio no responde");
                                Console.WriteLine($"Motivo {exception.Exception.Message}");
                                await Task.CompletedTask;
                            }
                         );

decimal dollarRate = await fallbackPolicy.ExecuteAsync(async () =>
{
    return await GetDollarRate();
});

Console.WriteLine($"La tasa del dólar es ${dollarRate}");

async Task<decimal> GetDollarRate()
{
    Console.WriteLine("Solicitud a servicio");
    throw new HttpRequestException();
}