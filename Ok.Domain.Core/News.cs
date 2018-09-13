using System;
using System.ComponentModel.DataAnnotations;

namespace Ok.Domain.Core
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }
    }
}
