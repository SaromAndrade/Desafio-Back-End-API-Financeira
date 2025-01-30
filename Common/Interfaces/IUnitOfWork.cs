using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        // Repositório genérico
        IRepository<T> Repository<T>() where T : class;

        IUserRepository UserRepository { get; }
        IWalletRepository WalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }

        Task<int> CompleteAsync();
    }
}
