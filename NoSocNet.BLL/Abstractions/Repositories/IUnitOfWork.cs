using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NoSocNet.BLL.Abstractions.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();

        int Save();
    }
}
