using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazetaRuReaderBackend
{
    public class Article
    {
        public string header { get; set; }
        public string description { get; set; }
        public DateTime pubDate { get; set; }
        public Uri mainLink { get; set; }
        public Uri imgLink { get; set; }

    }
}
