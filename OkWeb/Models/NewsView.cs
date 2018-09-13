using System.Collections.Generic;
using Ok.Domain.Core;

namespace OkWeb.Models
{
    public class NewsView
    {
        public List<Source> Sources { get; set; }
        public List<News> News { get; set; }
        public Pagination Pagination { get; set; }

        public int Source { get; set; }
        public string Sort { get; set; }
    }
}