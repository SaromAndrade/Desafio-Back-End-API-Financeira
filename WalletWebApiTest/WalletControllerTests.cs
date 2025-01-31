using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using WalletWebApi.Controllers;
using WalletWebApi.Models;
using WalletWebApi.Service;

namespace WalletWebApiTest
{
    public class WalletControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly WalletService _walletService;
        private readonly WalletController _controller;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;   

        public WalletControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _walletService = new WalletService(_unitOfWorkMock.Object);
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.WalletRepository).Returns(_walletRepositoryMock.Object);
            _controller = new WalletController(_unitOfWorkMock.Object, _walletService);
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _unitOfWorkMock.Setup(u => u.TransactionRepository).Returns(_transactionRepositoryMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenWalletExists()
        {
            // Arrange
            int userId = 1;
            var wallet = new Wallet { Id = 1, UserId = userId, Balance = 100 };

            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(wallet);

            // Act
            var result = await _controller.Get(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWallet = Assert.IsType<Wallet>(okResult.Value);
            Assert.Equal(userId, returnedWallet.UserId);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenWalletDoesNotExist()
        {
            // Arrange
            int userId = 99;

            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync((Wallet)null);

            // Act
            var result = await _controller.Get(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Converte o objeto para JSON, simulando o comportamento do navegador
            var json = JsonConvert.SerializeObject(notFoundResult.Value);

            // Esperado (mesmo JSON retornado pelo navegador)
            var expectedJson = JsonConvert.SerializeObject(new { Message = "Carteira não encontrada para este usuário." });

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public async Task CreateWallet_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock.Setup(repo => repo.ExistsAsync(userId))
                .ReturnsAsync(false); // Simula que o usuário não existe

            // Act
            var result = await _controller.CreateWallet(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonConvert.SerializeObject(notFoundResult.Value);
            var expectedJson = JsonConvert.SerializeObject(new { Message = "Usuário não encontrado." });

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public async Task CreateWallet_ReturnsBadRequest_WhenWalletAlreadyExists()
        {
            // Arrange
            int userId = 2;
            _userRepositoryMock.Setup(repo => repo.ExistsAsync(userId))
                .ReturnsAsync(true); // Usuário existe

            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(new Wallet { UserId = userId, Balance = 100 }); // Carteira já existe

            // Act
            var result = await _controller.CreateWallet(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var expectedJson = JsonConvert.SerializeObject(new { Message = "Usuário já possui uma carteira." });

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public async Task CreateWallet_ReturnsOk_WhenWalletIsCreatedSuccessfully()
        {
            // Arrange
            int userId = 3;
            _userRepositoryMock.Setup(repo => repo.ExistsAsync(userId))
                .ReturnsAsync(true); // Usuário existe

            _walletRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync((Wallet)null); // Usuário ainda não tem carteira

            // Simula adição da carteira e conclusão da transação
            _walletRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Wallet>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateWallet(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var wallet = Assert.IsType<Wallet>(okResult.Value);

            Assert.Equal(userId, wallet.UserId);
            Assert.Equal(0, wallet.Balance);
        }

        [Fact]
        public async Task Deposit_ReturnsNotFound_WhenWalletDoesNotExist()
        {
            // Arrange
            var request = new DepositRequest { UserId = 1, Amount = 100 };

            _walletRepositoryMock.Setup(w => w.GetByUserIdAsync(request.UserId)).ReturnsAsync((Wallet)null);

            // Act
            var result = await _controller.Deposit(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonConvert.SerializeObject(notFoundResult.Value);
            var expectedJson = JsonConvert.SerializeObject(new { Message = $"Carteira não encontrada para o usuário {request.UserId}." });

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public async Task Deposit_ReturnsBadRequest_WhenAmountIsZeroOrNegative()
        {
            // Arrange
            var request = new DepositRequest { UserId = 2, Amount = 0 };

            var wallet = new Wallet { Id = 10, UserId = request.UserId, Balance = 500 };

            _walletRepositoryMock.Setup(w => w.GetByUserIdAsync(request.UserId)).ReturnsAsync(wallet);
               
            // Act
            var result = await _controller.Deposit(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequestResult.Value);
            var expectedJson = JsonConvert.SerializeObject(new { Message = "O valor do depósito deve ser maior que zero." });

            Assert.Equal(expectedJson, json);
        }
        [Fact]
        public async Task Deposit_ReturnsOk_WhenDepositIsSuccessful()
        {
            // Arrange
            var request = new DepositRequest { UserId = 3, Amount = 200 };

            var wallet = new Wallet { Id = 20, UserId = request.UserId, Balance = 500 };

            _walletRepositoryMock.Setup(w => w.GetByUserIdAsync(request.UserId)).ReturnsAsync(wallet);


            _transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            //_unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deposit(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            var expectedJson = JsonConvert.SerializeObject(new { Message = "Depósito realizado com sucesso.", NewBalance = 700.0 });

            Assert.Equal(expectedJson, json);
        }
    }
}
