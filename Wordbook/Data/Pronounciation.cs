using System;

namespace Wordbook.Data
{
    public class Pronounciation:BaseEntity
    {
        public Pronounciation(DateTime registered, byte[] sound)
        {
            this.Registered = registered;
            this.Sound = sound;
        }
        public byte[] Sound { get; set; }
    }
}