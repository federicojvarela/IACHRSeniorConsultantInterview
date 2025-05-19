using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Servicio que proporciona operaciones básicas del sistema de archivos
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        /// <summary>
        /// Verifica si existe un archivo en la ruta especificada
        /// </summary>
        /// <param name="path">Ruta del archivo a verificar</param>
        /// <returns>True si el archivo existe, False en caso contrario</returns>
        public Task<bool> FileExistsAsync(string path) => Task.FromResult(File.Exists(path));

        /// <summary>
        /// Lee el contenido de un archivo de forma asíncrona
        /// </summary>
        /// <param name="path">Ruta del archivo a leer</param>
        /// <returns>Contenido del archivo como string</returns>
        public async Task<string> ReadFileAsync(string path) =>
            await File.ReadAllTextAsync(path);

        /// <summary>
        /// Escribe contenido en un archivo de forma asíncrona
        /// </summary>
        /// <param name="path">Ruta donde se escribirá el archivo</param>
        /// <param name="content">Contenido a escribir en el archivo</param>
        public async Task WriteFileAsync(string path, string content) =>
            await File.WriteAllTextAsync(path, content);

        /// <summary>
        /// Asegura que un directorio existe, creándolo si es necesario
        /// </summary>
        /// <param name="path">Ruta del directorio a verificar/crear</param>
        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

}
