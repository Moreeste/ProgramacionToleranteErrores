using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("MetalcodeApi", client =>
{
    client.BaseAddress = new Uri("https://metalcode.io");
}).AddPolicyHandler(GetRetryPolicy());

builder.Services.AddHttpClient("MetalcodeApiSimpleMode", client =>
{
    client.BaseAddress = new Uri("https://metalcode2.io");
}).AddTransientHttpErrorPolicy(policyBuilder =>
{
    return policyBuilder.WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(2), 
            onRetry: (response, time, attempt, context) =>
            {
                Console.WriteLine($"Intento número {attempt}");
            });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/checkstatus", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("MetalcodeApi");

    try
    {
        Console.WriteLine("Haciendo solicitud a api externa");

        var response = await client.GetAsync("cursos");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem($"Api externa falló con código {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();

        return Results.Ok($"Respuesta externa: {content}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al llamar api externa: {ex.Message}");
    }
});

app.MapGet("/checkstatusSimpleMode", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("MetalcodeApiSimpleMode");

    try
    {
        Console.WriteLine("Haciendo solicitud a api externa");

        var response = await client.GetAsync("cursos");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem($"Api externa falló con código {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();

        return Results.Ok($"Respuesta externa: {content}");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error al llamar api externa: {ex.Message}");
    }
});

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return Policy.Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                    onRetry: (result, time, attempt, context) =>
                    {
                        if (result.Exception != null)
                        {
                            Console.WriteLine($"Exception: {result.Exception.Message}");
                        }
                        else
                        {
                            Console.WriteLine($"Código HTTP fallido: {result.Result.StatusCode}");
                        }

                        Console.WriteLine($"Reintento {attempt} después de {time.TotalSeconds}s. \n");
                    }
                );
}