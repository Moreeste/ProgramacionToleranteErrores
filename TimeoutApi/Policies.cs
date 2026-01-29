using Polly;
using Polly.Timeout;

namespace TimeoutApi
{
    public class Policies
    {
        public IAsyncPolicy TimeOutPolicy(int seconds)
        {
            return Policy.TimeoutAsync(
                    TimeSpan.FromSeconds(seconds),
                    TimeoutStrategy.Optimistic,
                    onTimeoutAsync: (context, timespan, task, exception) =>
                    {
                        Console.WriteLine($"Se ha excedido el tiempo: {timespan.TotalSeconds}s");
                        return Task.CompletedTask;
                    }
                );
        }
    }
}
