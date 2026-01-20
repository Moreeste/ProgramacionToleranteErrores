var importResult = BeerImporter.Import("data.txt");

if (importResult.Notification.HasErrors)
{
    Console.WriteLine("Errores encontrados durante la importación:");
    foreach (var error in importResult.Notification.Errors)
    {
        Console.WriteLine(error);
    }
}

Console.WriteLine("-----------------------------");
Console.WriteLine("Cervezas con formato correcto:");
foreach (var beer in importResult.Beers)
{
    Console.WriteLine($"- {beer.Name}: ${beer.Price}");
}

public record Beer(string Name, decimal Price);
public record ImportResult(List<Beer> Beers, Notification Notification);

public static class BeerImporter
{
    public static ImportResult Import(string filePath)
    {
        var beers = new List<Beer>();
        var notification = new Notification();
        var fileData = File.ReadAllLines(filePath);

        for (int i = 1; i < fileData.Length; i++)
        {
            var rowErrors = new List<string>();
            var row = fileData[i].Split(',');

            if (row.Length != 2)
            {
                notification.Add($"Fila {i}: Número incorrecto de columnas");
                continue;
            }

            var nameText = row[0];
            var priceText = row[1];

            if (string.IsNullOrEmpty(nameText))
            {
                rowErrors.Add("El nombre de la cerveza no puede estar vacío");
            }

            if (!decimal.TryParse(priceText, out decimal price) || price <= 0)
            {
                rowErrors.Add("El precio debe ser un número decimal positivo");
            }

            if (rowErrors.Any())
            {
                notification.Add($"Fila {i}: {string.Join(", ", rowErrors)}");
                continue;
            }

            beers.Add(new Beer(nameText, price));
        }

        return new ImportResult(beers, notification);
    }
}

public class Notification
{
    private readonly List<string> _errors = new();
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();
    public bool HasErrors => _errors.Any();
    public void Add(string error)
    {
        _errors.Add(error);
    }
}