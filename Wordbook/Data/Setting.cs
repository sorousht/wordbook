using System.Collections.Generic;

namespace Wordbook.Data
{
    public class Setting
    {
        public IList<string> Databases { get; set; }

        public string CurrentDatabase { get; set; }
    }
}