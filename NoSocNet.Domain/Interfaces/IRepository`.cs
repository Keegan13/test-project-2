using System.Threading.Tasks;

namespace NoSocNet.Domain.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TKey> CreateAsync(TEntity entity);

        Task<TEntity> FindByIdAsync(TKey keys);
    }
}
