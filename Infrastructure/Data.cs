using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class FileDocumentStorage
    {
        private readonly string _basePath;
        private readonly Dictionary<Guid, Document> _documents;
        private readonly string _documentsFilePath;

        public FileDocumentStorage(string basePath)
        {
            _basePath = basePath;
            _documentsFilePath = Path.Combine(_basePath, "documents.json");

            Directory.CreateDirectory(_basePath);

            if (File.Exists(_documentsFilePath))
            {
                var json = File.ReadAllText(_documentsFilePath);
                _documents = JsonSerializer.Deserialize<Dictionary<Guid, Document>>(json)
                    ?? new Dictionary<Guid, Document>();
            }
            else
            {
                _documents = new Dictionary<Guid, Document>();
                SaveDocuments();
            }
        }

        public Document GetById(Guid id)
        {
            return _documents.ContainsKey(id) ? _documents[id] : null;
        }

        public List<Document> GetAll()
        {
            return new List<Document>(_documents.Values);
        }

        public Document Save(Document document)
        {
            _documents[document.Id] = document;
            SaveDocuments();
            return document;
        }

        public void Update(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                SaveDocuments();
            }
        }

        public void Delete(Guid id)
        {
            if (_documents.ContainsKey(id))
            {
                _documents.Remove(id);
                SaveDocuments();
            }
        }

        private void SaveDocuments()
        {
            var json = JsonSerializer.Serialize(_documents);
            File.WriteAllText(_documentsFilePath, json);
        }
    }
}
