using FluentValidation;
using FluentValidationApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<CustomerValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/customers", async (Customer customer, IValidator<Customer> validator) =>
{
    var validationResult = await validator.ValidateAsync(customer);

    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => new
        {
            e.PropertyName,
            e.ErrorMessage
        });

        return Results.BadRequest(errors);
    }

    return Results.Created($"/customers/{customer.Id}", customer);
});

app.Run();

