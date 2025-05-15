using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _catalogFilePath;
        private List<Catalog> _catalogs = new List<Catalog>();

        public CatalogRepository(string catalogFilePath)
        {
            _catalogFilePath = catalogFilePath;
            LoadCatalogs();
        }

        private void LoadCatalogs()
        {
            try
            {
                // Simulamos una carga lenta               
                Thread.Sleep(1000);

                // Leer el contenido del archivo
                var json = File.ReadAllText(_catalogFilePath);
                Console.WriteLine($"Contenido leído del archivo: {json}");

                // Opciones de deserialización
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                // Intento de deserialización
                _catalogs = JsonSerializer.Deserialize<List<Catalog>>(json, options) ?? new List<Catalog>();

                // Comprobación detallada del resultado
                if (_catalogs.Count == 0 || _catalogs.Any(c => string.IsNullOrEmpty(c.Id)))
                {
                    throw new InvalidOperationException("El archivo de catálogos está corrupto o los datos no son válidos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar catálogos: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Handle error (e.g., load default catalogs)
            }
        }

        public async Task<Catalog> GetCatalogByIdAsync(string id)
        {
            // Simulamos una consulta lenta
            await Task.Delay(500);
            return _catalogs.FirstOrDefault(c => c.Id == id) ?? new Catalog(); // Cambio para evitar null
        }

        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            // Simulamos una consulta lenta
            await Task.Delay(500);
            return _catalogs;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId)
        {
            // Simulamos una consulta lenta
            await Task.Delay(300);
            var catalog = _catalogs.FirstOrDefault(c => c.Id == catalogId);
            return catalog?.Items.FirstOrDefault(i => i.Id == itemId) ?? new CatalogItem(); // Cambio para evitar null  
        }
    }
}