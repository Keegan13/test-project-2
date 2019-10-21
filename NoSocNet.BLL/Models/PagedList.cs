using System.Collections.Generic;
using System.Linq;

namespace NoSocNet.BLL.Models
{
    public class PagedList<T>
    {

        public PagedList(IEnumerable<T> items, int totalCount, FilterBase filter, Paginator paginator) : this(items, totalCount)
        {
            this.Pagination = paginator;
            this.Filter = filter;
        }
        public PagedList(IEnumerable<T> items, int totalCount)
        {
            this.Items = items.ToArray();
            this.TotalCount = totalCount;
        }


        public FilterBase Filter { get; set; }

        public Paginator Pagination { get; set; }

        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }
}
