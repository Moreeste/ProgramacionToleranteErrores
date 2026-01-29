namespace TimeoutApi
{
    public class SlowOperation
    {
        public async Task Execute(CancellationToken token)
        {
            Console.WriteLine("Operación iniciada");

            await Task.Delay(TimeSpan.FromSeconds(5), token);

            Console.WriteLine("Operación finalizada");
        }
    }
}
