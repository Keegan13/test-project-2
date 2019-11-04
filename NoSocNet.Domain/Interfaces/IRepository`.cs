using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NoSocNet.Domain.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TKey> CreateAsync(TEntity entity);

        Task<ICollection<TEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10);

        Task<TEntity> FindByIdAsync(TKey keys);
    }
}
