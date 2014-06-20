using System;

namespace Wordbook.Data
{
    public class BaseEntity
    {
        public DateTime? Registered { get; set; }

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