using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        void MarkDelete(params object[] entities);
        void MarkDelete(IEnumerable<object> entities);
        void MarkCreate(params object[] entities);
        void MarkCreate(IEnumerable<object> entities);

        void MarkUpdate(params object[] entities);
        void MarkUpdate(IEnumerable<object> entities);
    }
}
