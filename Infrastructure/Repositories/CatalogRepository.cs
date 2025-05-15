using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio que maneja las operaciones relacionadas con los catálogos
    /// </summary>
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _catalogFilePath;
        private List<Catalog> _catalogs = new List<Catalog>();

        /// <summary>
        /// Constructor del repositorio de catálogos
        /// </summary>
        /// <param name="catalogFilePath">Ruta del archivo que contiene los catálogos</param>
        public CatalogRepository(string catalogFilePath)
        {
            _catalogFilePath = catalogFilePath;
            LoadCatalogs();
        }

        /// <summary>
        /// Carga los catálogos desde el archivo JSON
        /// </summary>
        private void LoadCatalogs()
        {
            try
            {
                // Simulamos una carga lenta para emular un escenario real               
                Thread.Sleep(1000);

                // Leer el contenido del archivo JSON
                var json = File.ReadAllText(_catalogFilePath);
                Console.WriteLine($"Contenido leído del archivo: {json}");

                // Configuración de las opciones de deserialización del JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Permite nombres de propiedades sin distinción de mayúsculas/minúsculas
                    AllowTrailingCommas = true, // Permite comas al final de las listas
                    ReadCommentHandling = JsonCommentHandling.Skip // Ignora los comentarios en el JSON
                };

                // Deserializa el JSON a una lista de catálogos
                _catalogs = JsonSerializer.Deserialize<List<Catalog>>(json, options) ?? new List<Catalog>();

                // Valida que los datos cargados sean correctos
                if (_catalogs.Count == 0 || _catalogs.Any(c => string.IsNullOrEmpty(c.Id)))
                {
                    throw new InvalidOperationException("El archivo de catálogos está corrupto o los datos no son válidos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar catálogos: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Maneja el error (por ejemplo, cargando catálogos por defecto)
            }
        }

        /// <summary>
        /// Obtiene un catálogo específico por su identificador
        /// </summary>
        /// <param name="id">Identificador único del catálogo</param>
        /// <returns>El catálogo encontrado o un catálogo vacío si no existe</returns>
        public async Task<Catalog> GetCatalogByIdAsync(string id)
        {
            // Simulamos una consulta lenta para emular un escenario real
            await Task.Delay(500);
            return _catalogs.FirstOrDefault(c => c.Id == id) ?? new Catalog();
        }

        /// <summary>
        /// Obtiene todos los catálogos disponibles
        /// </summary>
        /// <returns>Lista de todos los catálogos</returns>
        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            // Simulamos una consulta lenta para emular un escenario real
            await Task.Delay(500);
            return _catalogs;
        }

        /// <summary>
        /// Obtiene un elemento específico de un catálogo
        /// </summary>
        /// <param name="catalogId">Identificador del catálogo</param>
        /// <param name="itemId">Identificador del elemento</param>
        /// <returns>El elemento del catálogo encontrado o un elemento vacío si no existe</returns>
        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId)
        {
            // Simulamos una consulta lenta para emular un escenario real
            await Task.Delay(300);
            var catalog = _catalogs.FirstOrDefault(c => c.Id == catalogId);
            return catalog?.Items.FirstOrDefault(i => i.Id == itemId) ?? new CatalogItem();
        }
    }
}