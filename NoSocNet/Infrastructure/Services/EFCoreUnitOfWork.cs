
using NoSocNet.BLL.Abstractions.Repositories;
using NoSocNet.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Infrastructure.Services
{
    public class EFCoreUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        public EFCoreUnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }
        public int Save()
        {
            return context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
