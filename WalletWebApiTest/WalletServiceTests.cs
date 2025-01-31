using Core.Entities;
using Core.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using WalletWebApi.Models;
using WalletWebApi.Service;

namespace WalletWebApiTest
{
    public class WalletServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly WalletService _walletService;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IWalletRepository> _walletRepositoryMock;

        public WalletServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _walletService = new WalletService(_unitOfWorkMock.Object);
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _unitOfWorkMock.Setup(u => u.TransactionRepository)
            .Returns(_transactionRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.WalletRepository).Returns(_walletRepositoryMock.Object);

        }

        [Fact]
        public async Task GetWalletByUserIdAsync_ShouldThrowException_WhenWalletNotFound()
        {
            // Arrange
            var userId = 1;
            var walletRepositoryMock = new Mock<IWalletRepository>();
            walletRepositoryMock.Setup(w => w.GetByUserIdAsync(userId)).ReturnsAsync((Wallet)null);

            // Act
            Func<Task> act = async () => await _walletService.GetWalletByUserIdAsync(userId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Carteira não encontrada para o usuário {userId}.");
        }
        [Fact]
        public async Task GetUserTransfersAsync_ReturnsAllTransfers_WhenNoDateFilters()
        {
            // Arrange
            var userId = 1;
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-5) },
                new Transaction { Id = 2, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-3) }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(transactions);

            var request = new TransferFilterRequest { UserId = userId };

            // Act
            var result = await _walletService.GetUserTransfersAsync(request);

            // Assert
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public async Task GetUserTransfersAsync_ReturnsFilteredTransfers_WhenStartDateIsProvided()
        {
            // Arrange
            var userId = 1;
            var startDate = DateTime.Now.AddDays(-4);
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-5) },
                new Transaction { Id = 2, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-3) }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(transactions.Where(t => t.TransactionDate >= startDate));

            var request = new TransferFilterRequest { UserId = userId, StartDate = startDate };

            // Act
            var result = await _walletService.GetUserTransfersAsync(request);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.True(t.TransactionDate >= startDate));
        }
        [Fact]
        public async Task GetUserTransfersAsync_ReturnsFilteredTransfers_WhenEndDateIsProvided()
        {
            // Arrange
            var userId = 1;
            var endDate = DateTime.Now.AddDays(-4);
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-5) },
                new Transaction { Id = 2, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-3) }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(transactions.Where(t => t.TransactionDate <= endDate));

            var request = new TransferFilterRequest { UserId = userId, EndDate = endDate };

            // Act
            var result = await _walletService.GetUserTransfersAsync(request);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.True(t.TransactionDate <= endDate));
        }
        [Fact]
        public async Task GetUserTransfersAsync_ReturnsFilteredTransfers_WhenStartDateAndEndDateAreProvided()
        {
            // Arrange
            var userId = 1;
            var startDate = DateTime.Now.AddDays(-6);
            var endDate = DateTime.Now.AddDays(-4);
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-7) },
                new Transaction { Id = 2, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-5) },
                new Transaction { Id = 3, SenderUserId = userId, Type = TransactionType.Transfer, TransactionDate = DateTime.Now.AddDays(-3) }
            };

            _transactionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(transactions.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate));

            var request = new TransferFilterRequest { UserId = userId, StartDate = startDate, EndDate = endDate };

            // Act
            var result = await _walletService.GetUserTransfersAsync(request);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.True(t.TransactionDate >= startDate && t.TransactionDate <= endDate));
        }
        [Fact]
        public async Task GetUserTransfersAsync_ReturnsEmpty_WhenNoTransactionsMatchCriteria()
        {
            // Arrange
            var userId = 1;
            var startDate = DateTime.Now.AddDays(-10);
            var endDate = DateTime.Now.AddDays(-5);

            _transactionRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(new List<Transaction>()); // Nenhuma transação corresponde ao filtro

            var request = new TransferFilterRequest { UserId = userId, StartDate = startDate, EndDate = endDate };

            // Act
            var result = await _walletService.GetUserTransfersAsync(request);

            // Assert
            Assert.Empty(result);
        }
    }
}
