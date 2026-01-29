using Polly;
using Polly.Timeout;

var timeoutPolicy = Policy.TimeoutAsync(
        timeout: TimeSpan.FromSeconds(20),
        timeoutStrategy: TimeoutStrategy.Optimistic,
        onTimeoutAsync: (context, timespan, task, exception) =>
        {
            Console.WriteLine($"Mucho tiempo en la operación {timespan.TotalSeconds}s.");
            return Task.CompletedTask;
        }
    );

using var cancellationTokenSource = new CancellationTokenSource();

var task = Task.Run(async () =>
{
    try
    {
        await timeoutPolicy.ExecuteAsync(async (token) =>
        {
            //await Operation(TimeSpan.FromSeconds(10), token);
            await Execute(token);
        }, cancellationTokenSource.Token);
    }
    catch (TimeoutRejectedException)
    {
        Console.WriteLine("Excepción arrojada por polly por exceder el tiempo");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Operación cancelada manualmente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Otra excepción: {ex.GetType()} - {ex.Message}");
    }
});

await Task.Delay(5000);
Console.WriteLine("Se cancela manualmente");
cancellationTokenSource.Cancel();

await task;

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