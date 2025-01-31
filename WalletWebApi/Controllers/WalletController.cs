using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletWebApi.Models;
using WalletWebApi.Service;

namespace WalletWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WalletService _walletService;

        public WalletController(IUnitOfWork unitOfWork, WalletService walletService)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService; 
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] int userId)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                return NotFound(new { Message = "Carteira não encontrada para este usuário." });

            return Ok(wallet);
        }
        [HttpPost("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> CreateWallet(int userId)
        {
            // Verifica se o usuário existe
            var userExists = await _unitOfWork.UserRepository.ExistsAsync(userId);
            if (!userExists)
                return NotFound(new { Message = "Usuário não encontrado." });

            // Verifica se o usuário já tem uma carteira
            var existingWallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(userId);
            if (existingWallet != null)
                return BadRequest(new { Message = "Usuário já possui uma carteira." });

            // Cria a carteira
            var wallet = new Wallet { UserId = userId, Balance = 0 };

            await _unitOfWork.WalletRepository.AddAsync(wallet);
            await _unitOfWork.CompleteAsync();

            return Ok(wallet);
        }
        [HttpPost("deposit")]
        [Authorize]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            try
            {
                var wallet = await _walletService.GetWalletByUserIdAsync(request.UserId);

                if (request.Amount <= 0)
                    return BadRequest(new { Message = "O valor do depósito deve ser maior que zero." });

                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    WalletId = wallet.Id,
                    Type = TransactionType.Deposit,
                    TransactionDate = DateTime.UtcNow,
                    ReceiverUserId = request.UserId
                };

                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                wallet.Balance += request.Amount;
                await _unitOfWork.CompleteAsync();

                return Ok(new { Message = "Depósito realizado com sucesso.", NewBalance = wallet.Balance });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [HttpPost("transfer")]
        [Authorize]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            try
            {
                var senderWallet = await _walletService.GetWalletByUserIdAsync(request.SenderUserId);
                var receiverWallet = await _walletService.GetWalletByUserIdAsync(request.ReceiverUserId);

                if (request.Amount <= 0)
                    return BadRequest(new { Message = "O valor da transferência deve ser maior que zero." });

                if (senderWallet.Balance < request.Amount)
                    return BadRequest(new { Message = "Saldo insuficiente." });

                var transaction = new Transaction
                {
                    Amount = request.Amount,
                    WalletId = senderWallet.Id,
                    Type = TransactionType.Transfer,
                    TransactionDate = DateTime.UtcNow,
                    SenderUserId = request.SenderUserId,
                    ReceiverUserId = request.ReceiverUserId
                };

                senderWallet.Balance -= request.Amount;
                receiverWallet.Balance += request.Amount;

                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.CompleteAsync();

                return Ok(new
                {
                    Message = "Transferência realizada com sucesso.",
                    SenderNewBalance = senderWallet.Balance,
                    ReceiverNewBalance = receiverWallet.Balance
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [HttpPost("transfers/list")]
        [Authorize]
        public async Task<IActionResult> GetUserTransfers([FromBody] TransferFilterRequest request)
        {
            try
            {
                var transfers = await _walletService.GetUserTransfersAsync(request);
                return Ok(transfers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
