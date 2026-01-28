using CircuitBreakerApi;
using Polly.CircuitBreaker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("JsonApi", client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
}).AddPolicyHandler(Policies.GetCircuitBreakerPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/posts", async (IHttpClientFactory factory) =>
{
    var httpClient = factory.CreateClient("JsonApi");

    try
    {
        var response = await httpClient.GetAsync("posts");

        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem($"Código HTTP: {(int)response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        return Results.Ok(content);
    }
    catch (BrokenCircuitException)
    {
        return Results.Problem("Circuito abierto: el servicio está temporalmente bloqueado");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error inesperado: {ex.Message}");
    }
});

app.Run();
