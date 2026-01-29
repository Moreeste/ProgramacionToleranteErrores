using Polly;
using Polly.Timeout;

var timeoutPolicy = Policy.TimeoutAsync(
        timeout: TimeSpan.FromSeconds(4),
        timeoutStrategy: TimeoutStrategy.Optimistic,
        onTimeoutAsync: (context, timespan, task, exception) =>
        {
            Console.WriteLine($"Mucho tiempo en la operación {timespan.TotalSeconds}s.");
            return Task.CompletedTask;
        }
    );

try
{
    await timeoutPolicy.ExecuteAsync(async (token) =>
    {
        //await Operation(TimeSpan.FromSeconds(10), token);
        await Execute(token);
    }, CancellationToken.None);
}
catch (TimeoutRejectedException ex)
{
    Console.WriteLine("Excepción arrojada por polly por exceder el tiempo");
}
catch (Exception ex)
{
    Console.WriteLine($"Otra excepción: {ex.GetType()} - {ex.Message}");
}

Console.WriteLine("Observando durante 5 seg para ver si execute continúa");
await Task.Delay(5000);

async Task Operation(TimeSpan delay, CancellationToken token)
{
    Console.WriteLine("Haciendo algo...");
    await Task.Delay(delay, token);
}

async Task Execute(CancellationToken token)
{
    while (true)
    {
        Console.WriteLine("Haciendo algo");
        await Task.Delay(1000, token);
    }
}