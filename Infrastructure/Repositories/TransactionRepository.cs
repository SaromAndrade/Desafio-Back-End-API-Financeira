using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public IQueryable<Transaction> GetQueryable()
        {
            return _context.Transactions.AsQueryable();
        }

    }
}
