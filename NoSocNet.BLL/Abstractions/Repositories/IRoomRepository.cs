using NoSocNet.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IRoomRepositoryStore<TUser, TKey>
    {
        Task<ChatRoom<TUser, TKey>> GetPrivateRoom(TKey firstUserId, TKey secondUserId);
        Task<ChatRoom<TUser, TKey>> CreatePrivateRoom(TKey firstUserId, TKey secondUserId);
        Task<PagedList<ChatRoom<TUser, TKey>>> GetRoomsByUserAsync(TKey userId, FilterBase filter = null, Paginator pagination = null);
    }


    public class PagedList<T>
    {

        public PagedList(IEnumerable<T> items, int totalCount, FilterBase filter, Paginator paginator) : this(items, totalCount)
        {
            this.Pagination = paginator;
            this.Filter = filter;
        }
        public PagedList(IEnumerable<T> items, int totalCount)
        {
            this.Items = items;
            this.TotalCount = totalCount;
        }

        public FilterBase Filter { get; set; }

        public Paginator Pagination { get; set; }

        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
    }


    public class FilterBase
    {
        public string Keywords { get; set; }
    }

    public enum SortOrder
    {
        NotSpecified,
        Ascending,
        Descending
    }

    public class Paginator
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortColumn { get; set; }
        public SortOrder SortOrder { get; set; }
    }
}
