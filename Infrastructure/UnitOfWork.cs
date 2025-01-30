using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private IUserRepository _userRepository;
        private IWalletRepository _walletRepository;
        private ITransactionRepository _transactionRepository;  

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Implementação do repositório genérico
        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        // Implementação do repositório específico
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        public IWalletRepository WalletRepository => _walletRepository ??= new WalletRepository(_context);

        public ITransactionRepository TransactionRepository => _transactionRepository ??= new TransactionRepository(_context);

        // Método para salvar as alterações no banco de dados
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Método para liberar recursos (opcional)
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
