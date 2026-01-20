var climateGetters = new List<GetClimate>
{
    new GetClimate("40.4168", "-3.7038"), // Madrid
    new GetClimate("48.8566", "2.3522"),  // Paris
    new GetClimate("51.5074", "-0.1278"), // London
    new GetClimate("35.6895", "139.6917") // Tokyo
};

var getterInfo = new GetterInfo(climateGetters);
var notification = await getterInfo.ExecuteAllAsync();

if (notification.HasErrors)
{
    Console.WriteLine("Ocurrieron algunos errores:");
    foreach (var error in notification.Errors)
    {
        Console.WriteLine(error);
    }
}
else
{
    Console.WriteLine("Todas las llamadas a la API se realizaron con éxito.");
}

public class GetterInfo
{
    private readonly List<GetClimate> _climateGetters = new();
    public GetterInfo(List<GetClimate> climateGetter) => _climateGetters = climateGetter;
    public async Task<Notification> ExecuteAllAsync()
    {
        var notification = new Notification();

        foreach (var getter in _climateGetters)
        {
            var result = await getter.ExecuteAsync();
            if (result.HasErrors)
            {
                foreach (var error in result.Errors)
                {
                    notification.Add(error);
                }
            }
        }

        return notification;
    }
}

public class GetClimate
{
    private readonly HttpClient _http = new();
    private const string ApiUrl = "https://api.open-meteo.com/v1/forecast?current_weather=true";
    private string _lat;
    private string _lon;

    public GetClimate(string lat, string lon)
    {
        _lat = lat;
        _lon = lon;
    }

    public async Task<Notification> ExecuteAsync()
    {
        var notification = new Notification();

        try
        {
            var resp = await _http.GetAsync($"{ApiUrl}&latitude={_lat}&longitude={_lon}");

            if (!resp.IsSuccessStatusCode)
            {
                notification.Add($"Error en API ({_lat}, {_lon}): {resp.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            notification.Add($"Error: {ex.Message}");
        }

        return notification;
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