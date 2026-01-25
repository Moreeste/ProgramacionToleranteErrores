using ApiWithLogs;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Logging.AddSimpleConsole(options =>
//{
//    options.SingleLine = true;
//    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
//});

Log.Logger =  new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:w3}] {Message:lj} {RequestId}{NewLine}")
    .WriteTo.File(
        path: "Logs/log.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 10,
        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj} {Properties}{NewLine}"
     ).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/hi", (ILogger<Program> log) =>
{
    log.LogTrace("Se rastrea que entre al endpoint /hi");
    log.LogInformation("Entraron al endpoint {endpoint}", new { e = "/hi"});
    log.LogWarning("¡Cuidado! han entrado al endpoint /hi");

    return "Hola mundo";
});

app.MapPost("/checktotal", (Order order, ILogger<Program> log) =>
{
    try
    {
        log.LogInformation("Calculando total de la orden con {itemCount} items", order.Detail.Count);

        decimal total = 0;

        if (ProductRepository.Products.Count <= 0)
        {
            log.LogWarning("No hay productos en la db");
        }

        foreach (var item in order.Detail)
        {
            log.LogDebug("Buscando producto {productoId} con la cantidad {quantity}", item.IdProduct, item.Quantity);

            var product = ProductRepository.Products.FirstOrDefault(p => p.Id == item.IdProduct);
            if (product == null)
            {
                log.LogError("El producto {productoId} no existe", item.IdProduct);
                return Results.BadRequest($"El producto {item.IdProduct} no existe");
            }

            log.LogDebug("Producto encontrado: {productName} - Precio: {price}", product.Name, product.Price);

            total += product.Price * item.Quantity;
        }

        log.LogDebug("Total de la orden: {total}", total);
        return Results.Ok(total);
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Error al calcular todal de la orden");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

app.Run();

public record Order(List<OrderDetail> Detail);
public record OrderDetail(int IdProduct, int Quantity);
public record Product(int Id, string Name, decimal Price);