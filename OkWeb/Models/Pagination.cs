using System.Collections.Generic;

namespace OkWeb.Models
{
    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int SizePage { get; set; }
        public int TotalPages { get; set; }
        public int Source { get; set; }
        public string Sort { get; set; }
        public List<int> ViewPagination { get; set; }

        public Pagination()
        {
            ViewPagination = new List<int>();
        }
    }
}