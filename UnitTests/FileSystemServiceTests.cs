using Infrastructure.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    /// <summary>
    /// Pruebas unitarias para el servicio del sistema de archivos
    /// </summary>
    public class FileSystemServiceTests
    {
        private readonly FileSystemService _fileSystemService;
        private readonly string _testDirectory;

        /// <summary>
        /// Constructor que inicializa el servicio y el directorio de pruebas
        /// </summary>
        public FileSystemServiceTests()
        {
            _fileSystemService = new FileSystemService();
            _testDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
            _fileSystemService.EnsureDirectoryExists(_testDirectory);
        }

        /// <summary>
        /// Prueba que verifica que FileExists devuelve verdadero cuando el archivo existe
        /// </summary>
        [Fact]
        public void FileExists_ShouldReturnTrue_WhenFileExists()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");

            // Acción
            var result = _fileSystemService.FileExists(filePath);

            // Verificación
            Assert.True(result);
        }

        /// <summary>
        /// Prueba que verifica que FileExists devuelve falso cuando el archivo no existe
        /// </summary>
        [Fact]
        public void FileExists_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "nonexistentfile.txt");

            // Acción
            var result = _fileSystemService.FileExists(filePath);

            // Verificación
            Assert.False(result);
        }

        /// <summary>
        /// Prueba que verifica que ReadFileAsync devuelve el contenido correcto cuando el archivo existe
        /// </summary>
        [Fact]
        public async Task ReadFileAsync_ShouldReturnFileContent_WhenFileExists()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "testfile.txt");
            var expectedContent = "Test content";
            await _fileSystemService.WriteFileAsync(filePath, expectedContent);

            // Acción
            var result = await _fileSystemService.ReadFileAsync(filePath);

            // Verificación
            Assert.Equal(expectedContent, result);
        }

        /// <summary>
        /// Prueba que verifica que WriteFileAsync crea correctamente un archivo con contenido
        /// </summary>
        [Fact]
        public async Task WriteFileAsync_ShouldCreateFileWithContent()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "writefile.txt");
            var content = "Content to write";

            // Acción
            await _fileSystemService.WriteFileAsync(filePath, content);

            // Verificación
            Assert.True(File.Exists(filePath));
            Assert.Equal(content, await _fileSystemService.ReadFileAsync(filePath));
        }
    }
}