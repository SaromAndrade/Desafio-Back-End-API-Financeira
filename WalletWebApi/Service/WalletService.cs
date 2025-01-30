using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using WalletWebApi.Models;

namespace WalletWebApi.Service
{
    public class WalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Wallet> GetWalletByUserIdAsync(int userId)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new KeyNotFoundException($"Carteira não encontrada para o usuário {userId}.");

            return wallet;
        } 
        public async Task<IEnumerable<Transaction>> GetUserTransfersAsync(TransferFilterRequest request)
        {
            var wallet = await GetWalletByUserIdAsync(request.UserId);

            var query = _unitOfWork.TransactionRepository
                .GetQueryable()
                .Where(t => t.SenderUserId == request.UserId && t.Type == TransactionType.Transfer);

            if (request.StartDate.HasValue)
                query = query.Where(t => t.TransactionDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= request.EndDate.Value);

            return await query.ToListAsync();
        }
    }
}
