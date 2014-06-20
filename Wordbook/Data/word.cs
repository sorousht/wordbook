using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wordbook.Data
{
    public class Word:BaseEntity
    {
        public Word()
        {
            this.Registered = DateTime.Now;
        }

        public string Text { get; set; }
        public WordType Type { get; set; }
        public string Definition { get; set; }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
