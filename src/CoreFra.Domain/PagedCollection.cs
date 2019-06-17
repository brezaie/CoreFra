using System.Collections.Generic;

namespace CoreFra.Domain
{
    public class PagedCollection<T>
    {
        public long TotalCount { get; set; }
        public IEnumerable<T> List { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}