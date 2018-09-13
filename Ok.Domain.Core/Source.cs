using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ok.Domain.Core
{
    public class Source
    {
        [Key]
        public int SourceId  {get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public ICollection<News> News { get; set; }

        public Source()
        {
            News = new List<News>();
        }
    }
}
