using Polly;
using Polly.Timeout;

var timeoutPolicy = Policy.TimeoutAsync(
        timeout: TimeSpan.FromSeconds(2),
        timeoutStrategy: Polly.Timeout.TimeoutStrategy.Pessimistic,
        onTimeoutAsync: async (context, timespan, task, exception) =>
        {
            Console.WriteLine($"[{context["Id"]}] Mucho tiempo en la operación {timespan.TotalSeconds}");
            await Task.CompletedTask;
        }
    );

var context = new Context();
context["Id"] = "12345";

try
{
    await timeoutPolicy.ExecuteAsync(async (context) =>
    {
        await Operation(TimeSpan.FromSeconds(10));
    }, context);
}
catch (TimeoutRejectedException)
{
    Console.WriteLine("Excepción arrojada por polly por exceder el tiempo");
}
catch (Exception ex)
{
    Console.WriteLine($"Otra excepción: {ex.Message}");
}

async Task Operation(TimeSpan delay)
{
    Console.WriteLine("Haciendo algo...");
    await Task.Delay(delay);
}