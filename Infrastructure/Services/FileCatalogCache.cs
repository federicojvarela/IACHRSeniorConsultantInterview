using Microsoft.Extensions.Caching.Memory;
using Core.DTOs;
using Core.Interfaces;
using System.Text.Json;

/// <summary>
/// Implementación de ICatalogCache que utiliza archivos y caché en memoria para almacenar elementos de catálogo.
/// Supervisa cambios en el archivo de catálogos y actualiza la caché automáticamente.
/// </summary>
public class FileCatalogCache : ICatalogCache
{
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "CatalogCache";
    private readonly string _catalogDirectory;
    private readonly FileSystemWatcher _watcher;

    /// <summary>
    /// Inicializa una nueva instancia de FileCatalogCache y configura el watcher de archivos.
    /// </summary>
    /// <param name="catalogDirectory">Directorio donde se encuentra el archivo de catálogos.</param>
    /// <param name="cache">Instancia de IMemoryCache a utilizar.</param>
    public FileCatalogCache(string catalogDirectory, IMemoryCache cache)
    {
        _catalogDirectory = catalogDirectory;
        _cache = cache;

        _watcher = new FileSystemWatcher(_catalogDirectory)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _watcher.Changed += InvalidateCache;
        _watcher.Created += InvalidateCache;
        _watcher.Deleted += InvalidateCache;
        _watcher.Renamed += InvalidateCache;

        _watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Invalida la caché cuando se detecta un cambio en el archivo de catálogos.
    /// </summary>
    private void InvalidateCache(object sender, FileSystemEventArgs e)
    {
        _cache.Remove(_cacheKey);
    }

    /// <summary>
    /// Obtiene la lista de elementos del catálogo desde la caché o desde disco si no está cacheado.
    /// </summary>
    /// <returns>Lista de elementos del catálogo.</returns>
    public List<CatalogItemDto> GetCatalog()
    {
        if (_cache.Get(_cacheKey) is List<CatalogItemDto> cached)
            return cached;

        var catalog = LoadCatalogFromDisk();
        _cache.Set(_cacheKey, catalog, DateTimeOffset.Now.AddMinutes(10)); // Expiración configurable
        return catalog;
    }

    /// <summary>
    /// Carga los elementos del catálogo desde el archivo en disco.
    /// </summary>
    /// <returns>Lista de elementos del catálogo.</returns>
    private List<CatalogItemDto> LoadCatalogFromDisk()
    {
        var filePath = Path.Combine(_catalogDirectory, "catalogs.json");
        if (!File.Exists(filePath)) return new();

        var json = File.ReadAllText(filePath);
        var raw = JsonSerializer.Deserialize<List<CatalogDto>>(json);
        return raw?
            .SelectMany(cat => cat.Items.Select(item => new CatalogItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Value = item.Value
            }))
            .ToList() ?? new();
    }

    /// <summary>
    /// Obtiene la lista de elementos del catálogo de forma asíncrona.
    /// </summary>
    /// <returns>Lista de elementos del catálogo.</returns>
    public async Task<List<CatalogItemDto>?> GetCatalogAsync()
    {
        return await Task.FromResult(GetCatalog());
    }

    /// <summary>
    /// Fuerza la recarga de la caché del catálogo.
    /// </summary>
    public async Task RefreshAsync()
    {
        _cache.Remove(_cacheKey);
        await Task.CompletedTask;
    }
}
