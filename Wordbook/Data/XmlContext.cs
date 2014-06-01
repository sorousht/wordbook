﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class XmlContext
    {
        public XmlContext(string uri)
        {
            this.FilePath = uri;

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

        public IEnumerable<Word> GetWords(string keyword, DateTime date)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return from element in this.Document.Words().WordsList()
                       let word = element.AsWord()
                       where word.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
                             word.Registered > date
                       orderby word.Text ascending
                       select word;

            }
            else
            {
                return from element in this.Document.Words().WordsList()
                       let word = element.AsWord()
                       where word.Registered > date
                       orderby word.Text ascending
                       select word;

            }

        }

        private XElement GetWord(DateTime registered)
        {
            return this.Document.Words()
                .WordsList()
                .FirstOrDefault(element => element.AsWord().Registered == registered);
        }

        public void AddWord(Word word)
        {
            word.Registered = DateTime.Now;

            this.Document.Words().Add(word.ToXElement());

            this.Document.Save(FilePath);
        }

        public void UpdateWord(Word word)
        {
            var element = word.Registered.HasValue ? this.GetWord(word.Registered.Value) : null;

            if (element != null)
            {
                element.ReplaceWith(word.ToXElement());
                this.Document.Save(FilePath);
            }

        }

        public void Remove(Word word)
        {
            var element = word.Registered.HasValue ? this.GetWord(word.Registered.Value) : null;

            if (element != null)
            {
                element.Remove();

                this.Document.Save(this.FilePath);
            }
        }

        public bool Exists(Word word)
        {
            return this.Document.Words()
                .WordsList()
                .Any(element => element.AsWord().Registered == word.Registered);
        }
    }
}
