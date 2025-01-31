using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
            Expression<Func<Transaction, bool>> filter = t => t.SenderUserId == request.UserId && t.Type == TransactionType.Transfer;

            if (request.StartDate.HasValue)
                filter = filter.And(t => t.TransactionDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                filter = filter.And(t => t.TransactionDate <= request.EndDate.Value);

            return await _unitOfWork.TransactionRepository.GetAllAsync(filter);
        }
    }
}
