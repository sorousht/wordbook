using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class WordService
    {
        public WordService(string uri)
        {
            this.FilePath = uri;

            if (!File.Exists(this.FilePath))
            {
                this.CreateFile(this.FilePath);
            }

            var content = File.ReadAllText(this.FilePath);

            this.Words = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<Word>>(content);
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

        private IList<Word> Words { get; set; }

        public IEnumerable<Word> Search(string keyword, DateTime date)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return from word in this.Words
                       where word.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
                             word.Registered > date
                       orderby word.Text ascending
                       select word;

            }
            else
            {
                return from word in this.Words
                       where word.Registered > date
                       orderby word.Text ascending
                       select word;

            }

        }

        private Word GetWord(DateTime registered)
        {
            return this.Words.FirstOrDefault(word => word.Registered == registered);
        }

        public void AddWord(Word word)
        {
            word.Registered = DateTime.Now;

            this.Words.Add(word);

            this.Save();
        }

        public void Remove(Word word)
        {
            this.Words.Remove(word);

            this.Save();
        }

        public void Save()
        {
            File.WriteAllText(this.FilePath, Newtonsoft.Json.JsonConvert.SerializeObject(this.Words));
        }

        public bool Exists(Word word)
        {
            return this.Words.Any(item => item.Equals(word));
        }
    }
}
