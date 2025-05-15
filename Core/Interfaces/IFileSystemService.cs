using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Define una interfaz para operaciones del sistema de archivos
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Verifica si existe un archivo en la ruta especificada
        /// </summary>
        /// <param name="path">Ruta del archivo a verificar</param>
        /// <returns>Verdadero si el archivo existe, falso en caso contrario</returns>
        bool FileExists(string path);

        /// <summary>
        /// Lee el contenido de un archivo de forma asíncrona
        /// </summary>
        /// <param name="path">Ruta del archivo a leer</param>
        /// <returns>El contenido del archivo como una cadena de texto</returns>
        Task<string> ReadFileAsync(string path);

        /// <summary>
        /// Escribe contenido en un archivo de forma asíncrona
        /// </summary>
        /// <param name="path">Ruta donde se escribirá el archivo</param>
        /// <param name="content">Contenido a escribir en el archivo</param>
        Task WriteFileAsync(string path, string content);

        /// <summary>
        /// Asegura que el directorio especificado existe, creándolo si es necesario
        /// </summary>
        /// <param name="path">Ruta del directorio a verificar o crear</param>
        void EnsureDirectoryExists(string path);
    }

}
