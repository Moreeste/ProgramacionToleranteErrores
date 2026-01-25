using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        options.SingleLine = true;
    }).SetMinimumLevel(LogLevel.Trace);
});

var logger = loggerFactory.CreateLogger<Program>();

logger.LogTrace("Nivel trace");
logger.LogDebug("Nivel debug");
logger.LogInformation("Nivel información");
logger.LogWarning("Nivel advertencia");
logger.LogError("Nivel error");
logger.LogCritical("Nivel crítico");

loggerFactory.Dispose();