using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileSystemService
    {
        bool FileExists(string path);
        Task<string> ReadFileAsync(string path);
        Task WriteFileAsync(string path, string content);
        void EnsureDirectoryExists(string path);
    }

}
