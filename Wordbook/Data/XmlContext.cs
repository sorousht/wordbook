using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class XmlContext:IDisposable
    {
        public XmlContext(string uri)
        {
            this.FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri);
            if (!File.Exists(this.FilePath))
            {
                this.CreateFile(this.FilePath);
            }

            this.Document = XDocument.Load(this.FilePath, LoadOptions.None);
        }

        private XDocument Document { get; set; }

        private string FilePath { get; set; }

        private void CreateFile(string path)
        {
            using (var file = File.CreateText(path))
            {
                file.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Wordbook><Words></Words></Wordbook>");
                file.Flush();
            }
        }

        public IEnumerable<Word> GetWords(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return this.Document.Words().WordsList()
                    .Where(word => word.AsWord().Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                    .SelectAsWord();

            }
            else
            {
                return this.Document.Words().WordsList().SelectAsWord();

            }

        }

        private XElement GetWord(string text)
        {
            return this.Document.Words().WordsList().FirstOrDefault(element => this.WordsAreEqual(element.AsWord().Text, text));
        }

        private bool WordsAreEqual(string first, string second)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        public void SaveWord(Word word)
        {
            var element = this.GetWord(word.Text);
            if (element != null)
            {
                element.ReplaceWith(word.ToXElement());
            }
            else
            {
                this.Document.Words().Add(word.ToXElement());
            }


        }

        public void Dispose()
        {
            this.Document.Save(this.FilePath);
        }
    }
}
