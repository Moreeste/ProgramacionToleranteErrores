using Polly;

namespace CircuitBreakerApi
{
    public static class Policies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy<HttpResponseMessage>.HandleResult(r => !r.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(10),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine($"Circuito ABIERTO por {timespan.TotalSeconds}s. Motivo: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    },
                    onReset: () => Console.WriteLine("Circuito CERRADO: conexión estable."),
                    onHalfOpen: () => Console.WriteLine("HALF-OPEN: probando nuevamente.")
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> GetAdvancedCircuitBreakerPolicy()
        {
            return Policy<HttpResponseMessage>.HandleResult(r => !r.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 6,
                    durationOfBreak: TimeSpan.FromSeconds(10),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine($"Circuito ABIERTO por {timespan.TotalSeconds}s. Motivo: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    },
                    onReset: () => Console.WriteLine("Circuito CERRADO: conexión estable."),
                    onHalfOpen: () => Console.WriteLine("HALF-OPEN: probando nuevamente.")
                );
        }
    }
}
