using Polly.Timeout;
using TimeoutApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<Policies>();
builder.Services.AddTransient<SlowOperation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("timeout", async (Policies policies, SlowOperation slowOperation) =>
{
    try
    {
        await policies.TimeOutPolicy(20).ExecuteAsync(async (cancellationToken) =>
        {
            await slowOperation.Execute(cancellationToken);
        }, CancellationToken.None);

        return Results.Ok("Operación completada exitosamente.");
    }
    catch (TimeoutRejectedException)
    {
        return Results.Problem("La operación excedió el tiempo permitido.");
    }
});

app.Run();
