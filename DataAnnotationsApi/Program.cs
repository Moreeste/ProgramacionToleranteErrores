using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/products", (Product product) =>
{
    var validationContext = new ValidationContext(product);
    var validationResults = new List<ValidationResult>();
    bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, true);

    if (!isValid)
    {
        return Results.BadRequest(validationResults);
    }

    return Results.Created($"/products/{product.Id}", product);
});

app.Run();

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre no debe superar los 100 caracteres")]
    public string Name { get; set; }


    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 10000.00, ErrorMessage = "El precio debe estar entre 0.01 y 10000.00")]
    public decimal? Price { get; set; }
}