using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool FileExists(string path) => File.Exists(path);

        public async Task<string> ReadFileAsync(string path) =>
            await File.ReadAllTextAsync(path);

        public async Task WriteFileAsync(string path, string content) =>
            await File.WriteAllTextAsync(path, content);

        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

}
