using System;
namespace Core.Interfaces
{
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
}