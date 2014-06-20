using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wordbook.Data
{
    public class JsonListService<T> where T : BaseEntity
    {
        public JsonListService(string uri)
        {
            this.FilePath = uri;

            if (!File.Exists(this.FilePath))
            {
                this.CreateFile(this.FilePath);
            }

            var content = File.ReadAllText(this.FilePath);

            this.Items = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<T>>(content);
        }

        private string FilePath { get; set; }

        private void CreateFile(string path)
        {
            using (var file = File.CreateText(path))
            {
                file.WriteLine("[]");
                file.Flush();
            }
        }

        internal IList<T> Items { get; set; }

        public T Get(DateTime registered)
        {
            return this.Items.SingleOrDefault(item => item.Registered == registered);
        }

        public bool Exists(DateTime registered)
        {
            return this.Items.Any(item => item.Registered == registered);
        }

        public bool Exists(T item)
        {
            return this.Items.Any(i => i.Equals(item));
        }

        public void Save()
        {
            File.WriteAllText(this.FilePath, Newtonsoft.Json.JsonConvert.SerializeObject(this.Items));
        }
    }
}