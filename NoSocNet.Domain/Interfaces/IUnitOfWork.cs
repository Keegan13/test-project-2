using System.Threading.Tasks;

namespace NoSocNet.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        void MarkDelete<T>(params T[] entities);
        void MarkCreate<T>(params T[] entities);
        void MarkUpdate<T>(params T[] entities);
    }
}
