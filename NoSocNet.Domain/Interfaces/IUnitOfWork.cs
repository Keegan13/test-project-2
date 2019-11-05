using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        void MarkDelete<T>(params T[] entities);
        void MarkCreate(params object[] entities);
        void MarkCreate(IEnumerable<object> entities);
        void MarkUpdate<T>(params T[] entities);
    }
}
