using Polly;
using System.Text.Json;

var fileName = "posts.json";
var url ="https://jsonplaceholder2.typicode.com/posts";

var fallbackPolicy = Policy<List<Post>>.Handle<HttpRequestException>()
						.FallbackAsync(
							fallbackValue: await GetInfoByFile(fileName),
							onFallbackAsync: (exception, context) =>
							{
                                Console.WriteLine("Fallback: servicio no response");
                                Console.WriteLine($"Motivo {exception.Exception.Message}");
                                Console.WriteLine("Se usará la información de respaldo");
								return Task.CompletedTask;
                            }
						);

var retryPolicy = Policy<List<Post>>.Handle<HttpRequestException>()
						.WaitAndRetryAsync(
							retryCount: 3,
							sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
							onRetry: (outcome, timeSpan, retryCount, context) =>
							{
								Console.WriteLine($"Retry {retryCount} en {timeSpan.TotalSeconds} segundos");
								Console.WriteLine($"Motivo: {outcome.Exception?.Message}");
							}
						);

var combinedPolicies = Policy.WrapAsync(fallbackPolicy, retryPolicy);

var posts = await combinedPolicies.ExecuteAsync(async () =>
{
    Console.WriteLine("Solicitud al servicio");

    var httpClient = new HttpClient();
	var response = await httpClient.GetAsync(url);
	response.EnsureSuccessStatusCode();
	var content =  await response.Content.ReadAsStringAsync();
	var posts = JsonSerializer.Deserialize<List<Post>>(content, new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
    });

    Console.WriteLine("Solicitud al servicio con éxito");
	
	var fileContent = JsonSerializer.Serialize(posts, new JsonSerializerOptions
	{
		WriteIndented = true
	});
	await File.WriteAllTextAsync(fileName, fileContent);

    return posts;
});

posts.ForEach(post =>
{
	Console.WriteLine($"Post {post.Id}: {post.Title}");
}) ;

async Task<List<Post>> GetInfoByFile(string fileName)
{
	if (!File.Exists(fileName))
	{
		return new List<Post>();
	}

	var fileContent = await File.ReadAllTextAsync(fileName);
	var posts = JsonSerializer.Deserialize<List<Post>>(fileContent);
	return posts;
}

record Post(int UserId, int Id, string Title, string Body);