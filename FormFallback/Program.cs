using Polly;
using System.Diagnostics;

namespace FormFallback
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var fallback = Policy<decimal>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: 18.5m,
                    onFallbackAsync: async b =>
                    {
                        await Task.Run(() =>
                        {
                            Debug.WriteLine("No se pudo obtener el tipo de cambio, se usará el valor por defecto");
                        });
                    }
                );

            var dollarRate = new DollarRate(fallback);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(dollarRate));
        }
    }
}