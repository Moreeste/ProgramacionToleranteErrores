using Polly;
using System.Net.NetworkInformation;

string host = "metalcode.com";

var pingPolicy = Policy.Handle<PingException>()
                        .OrResult<PingReply>(r => r.Status != IPStatus.Success)
                        .WaitAndRetryForeverAsync(
                            sleepDurationProvider: attempt => TimeSpan.FromSeconds(3),
                            onRetry: (result, attempt, time) =>
                            {
                                string error = result.Exception != null ? 
                                                result.Exception.Message : 
                                                result.Result.Status.ToString();

                                Console.WriteLine($"Reintento {attempt}. Error causado: {error}. Reintentando en {time.TotalSeconds} segundos.");
                            }
                         );

var context = new Context("Ping");
context["Host"] = host;

await pingPolicy.ExecuteAsync(async (context) =>
{
    using var ping = new Ping();
    Console.WriteLine($"Enviando ping a [{context["Host"]}]");

    var reply = await ping.SendPingAsync((string)context["Host"], 2000);

    if (reply.Status == IPStatus.Success)
    {
        Console.WriteLine($"Respuesta recivida desde {reply.Address} en {reply.RoundtripTime}ms.");
    }
    else
    {
        Console.WriteLine($"No hay una respuesta {reply.Status}");
    }

    return reply;

}, context);