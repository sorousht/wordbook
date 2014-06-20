using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class WordService : JsonListService<Word>
    {
        public WordService(string uri) : base(uri)
        {

        }
        public IEnumerable<Word> Search(string keyword, DateTime date)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return from word in this.Items
                       where word.Text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
                             word.Registered > date
                       orderby word.Text ascending
                       select word;

            }
            else
            {
                return from word in this.Items
                       where word.Registered > date
                       orderby word.Text ascending
                       select word;

            }

        }
    }
}
