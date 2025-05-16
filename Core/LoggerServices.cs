using Microsoft.Extensions.Logging;

/// <summary>
/// Servicio de logging personalizado para la aplicación.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Registra un mensaje informativo.
    /// </summary>
    /// <param name="message">Mensaje a registrar.</param>
    void LogInformation(string message);

    /// <summary>
    /// Registra un mensaje de error.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    void LogError(string message);

    /// <summary>
    /// Registra un mensaje de error junto con una excepción.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    /// <param name="ex">Excepción asociada.</param>
    void LogError(string message, Exception ex);

    /// <summary>
    /// Registra un mensaje de advertencia.
    /// </summary>
    /// <param name="message">Mensaje de advertencia.</param>
    void LogWarning(string message);
}

/// <summary>
/// Implementación de ILoggerService que utiliza Microsoft.Extensions.Logging.
/// </summary>
public class LoggerServices : ILoggerService
{
    private readonly ILogger<LoggerServices> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de logging.
    /// </summary>
    /// <param name="logger">Instancia de ILogger.</param>
    public LoggerServices(ILogger<LoggerServices> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void LogInformation(string message) => _logger.LogInformation(message);
    /// <inheritdoc/>
    public void LogError(string message) => _logger.LogError(message);
    /// <inheritdoc/>
    public void LogError(string message, Exception ex) => _logger.LogError(ex, message);
    /// <inheritdoc/>
    public void LogWarning(string message) => _logger.LogWarning(message);
}