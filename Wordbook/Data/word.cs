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
            
        }
        public Word(string text, string type, string definition, string registered)
        {
            this.Text = text;
            this.Type = (WordType)Enum.Parse(typeof(WordType), type);
            this.Definition = definition;
            this.Registered = Convert.ToDateTime(registered);
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

    public enum WordType
    {
        Noun = 1,
        Adjective = 2,
        Verb = 3,
        Adverb = 4,
        Clause = 5,
        Conjunction = 6,
        Infinitive = 7,
        Preposition = 8,
        Pronoun = 9,
        Sentence = 10,
        Idiom = 11,
    }
}
