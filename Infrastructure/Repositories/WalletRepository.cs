using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
