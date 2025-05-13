using Microsoft.Extensions.Logging;

public interface ILoggerService
{
    void LogInformation(string message);
    void LogError(string message);
    void LogError(string message, Exception ex);
    void LogWarning(string message);
}

public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message) => _logger.LogInformation(message);
    public void LogError(string message) => _logger.LogError(message);
    public void LogError(string message, Exception ex) => _logger.LogError(ex, message);
    public void LogWarning(string message) => _logger.LogWarning(message);
}