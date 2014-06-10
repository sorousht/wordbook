using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class Word
    {
        public Word()
        {
            this.Registered = DateTime.Now;
        }

        public string Text { get; set; }
        public WordType Type { get; set; }
        public string Definition { get; set; }
        public DateTime? Registered { get; set; }

        public override string ToString()
        {
            return this.Text;
        }

        public override int GetHashCode()
        {
            return this.Registered.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var word = obj as Word;
            if (word != null)
            {
                return this.Registered == word.Registered;
            }

            return false;
        }
    }
}
