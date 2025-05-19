using Microsoft.Extensions.Logging;
namespace UnitTests
{
    public class LoggerServicesTests
    {
        /// <summary>
        /// Mock para ILogger de Microsoft
        /// </summary>
        private readonly Mock<ILogger<LoggerServices>> _mockLogger;
        /// <summary>
        /// Instancia del servicio a testear
        /// </summary>
        private readonly LoggerServices _loggerServices;

        /// <summary>
        /// Inicializa el mock y el servicio para los tests
        /// </summary>
        public LoggerServicesTests()
        {
            _mockLogger = new Mock<ILogger<LoggerServices>>();
            _loggerServices = new LoggerServices(_mockLogger.Object);
        }

        /// <summary>
        /// Verifica que LogInformation llama a ILogger con LogLevel.Information
        /// </summary>
        [Fact]
        public void LogInformation_CallsILoggerLogInformation()
        {
            var message = "info message";
            _loggerServices.LogInformation(message);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        /// <summary>
        /// Verifica que LogError llama a ILogger con LogLevel.Error
        /// </summary>
        [Fact]
        public void LogError_CallsILoggerLogError()
        {
            var message = "error message";
            _loggerServices.LogError(message);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        /// <summary>
        /// Verifica que LogError con excepción llama a ILogger con LogLevel.Error y la excepción
        /// </summary>
        [Fact]
        public void LogError_WithException_CallsILoggerLogErrorWithException()
        {
            var message = "error with exception";
            var ex = new System.Exception("ex");
            _loggerServices.LogError(message, ex);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                ex,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        /// <summary>
        /// Verifica que LogWarning llama a ILogger con LogLevel.Warning
        /// </summary>
        [Fact]
        public void LogWarning_CallsILoggerLogWarning()
        {
            var message = "warn message";
            _loggerServices.LogWarning(message);
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
} 