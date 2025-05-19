using Infrastructure.Services;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
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
        public async Task FileExistsShouldReturnTrueWhenFileExists()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");

            // Acción
            var result = await _fileSystemService.FileExistsAsync(filePath);

            // Verificación
            Assert.True(result);
        }

        /// <summary>
        /// Prueba que verifica que FileExists devuelve falso cuando el archivo no existe
        /// </summary>
        [Fact]
        public async Task FileExistsShouldReturnFalseWhenFileDoesNotExist()
        {
            // Preparación
            var filePath = Path.Combine(_testDirectory, "nonexistentfile.txt");

            // Acción
            var result = await _fileSystemService.FileExistsAsync(filePath);

            // Verificación
            Assert.False(result);
        }

        /// <summary>
        /// Prueba que verifica que ReadFileAsync devuelve el contenido correcto cuando el archivo existe
        /// </summary>
        [Fact]
        public async Task ReadFileAsyncShouldReturnFileContentWhenFileExists()
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
        public async Task WriteFileAsyncShouldCreateFileWithContent()
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

        /// <summary>
        /// Prueba que verifica que ReadFileAsync lanza una excepción cuando el archivo no existe
        /// </summary>
        [Fact]
        public async Task ReadFileAsyncShouldThrowWhenFileDoesNotExist()
        {
            var filePath = Path.Combine(_testDirectory, "doesnotexist.txt");
            await Assert.ThrowsAsync<FileNotFoundException>(() => _fileSystemService.ReadFileAsync(filePath));
        }

        /// <summary>
        /// Prueba que verifica que WriteFileAsync sobrescribe correctamente un archivo existente
        /// </summary>
        [Fact]
        public async Task WriteFileAsyncShouldOverwriteExistingFile()
        {
            var filePath = Path.Combine(_testDirectory, "overwrite.txt");
            await _fileSystemService.WriteFileAsync(filePath, "First");
            await _fileSystemService.WriteFileAsync(filePath, "Second");
            var content = await _fileSystemService.ReadFileAsync(filePath);
            Assert.Equal("Second", content);
        }

        /// <summary>
        /// Prueba que verifica que EnsureDirectoryExists crea correctamente un directorio si falta
        /// </summary>
        [Fact]
        public void EnsureDirectoryExistsShouldCreateDirectoryIfMissing()
        {
            var dir = Path.Combine(_testDirectory, "newdir");
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
            _fileSystemService.EnsureDirectoryExists(dir);
            Assert.True(Directory.Exists(dir));
        }
    }
}