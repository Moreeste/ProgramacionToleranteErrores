using Polly;

var httpClient = new HttpClient();

var retryPolicy = Policy.Handle<HttpRequestException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        //sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
        //sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt * 2),
        //sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Random.Shared.Next(3, 8)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"[{context.OperationKey} de {context["endpoint"]}] Intento {retryCount} después de {timeSpan.TotalSeconds} segundos: {exception.Message}");
        });

var context = new Context("HttpOperation");
context["endpoint"] = "some.io";

try
{
    await retryPolicy.ExecuteAsync(async (context) =>
    {
        var response = await httpClient.GetAsync("https://somesite.io");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine("ORespuesta recibida con éxito");
        Console.WriteLine(content);
    }, context
    //new Context("HttpOperation")
    );
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Operación fallida después de reintentos: {ex.Message}");
}