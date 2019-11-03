using Microsoft.EntityFrameworkCore;
using NoSocNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Domain
{
    public class EFCoreRepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IIdentifiable<TKey>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ApplicationDbContext context;
        public EFCoreRepositoryBase(ApplicationDbContext context)
        {
            this.context = context;
            this.unitOfWork = context;

        }

        public async Task<TKey> CreateAsync(TEntity entity)
        {
            var result = await this.context.AddAsync(entity);
            await this.context.SaveChangesAsync();
            return (result as IIdentifiable<TKey>).Id;
        }

        public async Task<TEntity> FindByIdAsync(TKey keys)
        {
            return await this.context.Set<TEntity>().FindAsync(keys);
        }
    }
}
