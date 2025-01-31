using Core.Entities;
using Core.Interfaces;
using Moq;

namespace Infrastructure.Test
{
    public class WalletRepositoryTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock;

        public WalletRepositoryTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
        }
        [Fact]
        public async Task AddAsync_ShouldCallAddMethod()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, UserId = 10, Balance = 100.00m };

            // Act
            await _walletRepositoryMock.Object.AddAsync(wallet);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.AddAsync(wallet), Times.Once);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnWallet()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, UserId = 10, Balance = 100.00m };
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(wallet);

            // Act
            var result = await _walletRepositoryMock.Object.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wallet.Id, result.Id);
            Assert.Equal(wallet.Balance, result.Balance);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnWalletList()
        {
            // Arrange
            var wallets = new List<Wallet>
            {
                new Wallet { Id = 1, UserId = 10, Balance = 100.00m },
                new Wallet { Id = 2, UserId = 20, Balance = 50.00m }
            };

            _walletRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(wallets);

            // Act
            var result = await _walletRepositoryMock.Object.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public void Update_ShouldCallUpdateMethod()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, UserId = 10, Balance = 200.00m };

            // Act
            _walletRepositoryMock.Object.Update(wallet);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.Update(wallet), Times.Once);
        }
        [Fact]
        public void Delete_ShouldCallDeleteMethod()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, UserId = 10, Balance = 100.00m };

            // Act
            _walletRepositoryMock.Object.Delete(wallet);

            // Assert
            _walletRepositoryMock.Verify(repo => repo.Delete(wallet), Times.Once);
        }
        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnWallet_WhenUserExists()
        {
            // Arrange
            var wallet = new Wallet { Id = 1, UserId = 10, Balance = 100.00m };
            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(10)).ReturnsAsync(wallet);

            // Act
            var result = await _walletRepositoryMock.Object.GetByUserIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wallet.UserId, result.UserId);
            Assert.Equal(wallet.Balance, result.Balance);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(99)).ReturnsAsync((Wallet?)null);

            // Act
            var result = await _walletRepositoryMock.Object.GetByUserIdAsync(99);

            // Assert
            Assert.Null(result);
        }
    }
}
