using Microsoft.EntityFrameworkCore;
using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Domain
{
    public abstract class EFCoreRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IIdentifiable<TKey>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ApplicationDbContext context;
        public EFCoreRepositoryBase(ApplicationDbContext context)
        {
            this.context = context;
            this.unitOfWork = context;

        }

        public virtual async Task<TKey> CreateAsync(TEntity entity)
        {
            var result = this.context.Add(entity);
            await this.context.SaveChangesAsync();
            return result.Entity.Id;
        }

        public virtual async Task<TEntity> FindByIdAsync(TKey keys)
        {
            return await this.context.Set<TEntity>().FindAsync(keys);
        }


        public abstract Task<ICollection<TEntity>> Search(string keywords, string currentUserId, int skip = 0, int take = 10);
    }
}
