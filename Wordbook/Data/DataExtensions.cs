using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public static class DataExtensions
    {
        private static readonly int MaxItemsCount = 999;
        public static IEnumerable<Word> SelectAsWord(this IEnumerable<XElement> elements)
        {
            return elements.Select(word => new Word(
                        word.Element("Text").Value,
                        word.Attribute("Type").Value,
                        word.Element("Definition").Value,
                        word.Attribute("Registered").Value));
        }

        public static Word AsWord(this XElement element)
        {
            return new Word(
                element.Element("Text").Value,
                element.Attribute("Type").Value,
                element.Element("Definition").Value,
                element.Attribute("Registered").Value);
        }

        public static XElement ToXElement(this Word word)
        {
            return new XElement("Word",
                new XAttribute("Type", word.Type),
                new XAttribute("Registered", word.Registered),
                new XElement("Text", word.Text),
                new XElement("Definition", word.Definition));
        }

        public static IEnumerable<XElement> WordsList(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return element.Elements("Word").Take(DataExtensions.MaxItemsCount);
        }

        public static XElement Words(this XDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var wordbook = document.Element("Wordbook");

            if (wordbook == null)
            {
                throw new Exception("The \"Wordbook\" elemnt was not found");
            }

            var words = wordbook.Element("Words");

            if (words == null)
            {
                throw new Exception("Could not found \"Words\" element in \"Wordbook\"");
            }

            return words;

        }
    }
}