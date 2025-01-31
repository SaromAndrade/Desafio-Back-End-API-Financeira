using Core;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Infrastructure.Test
{
    public class UserRepositoryTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<DbContext> _dbContextMock;

        public UserRepositoryTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _dbContextMock = new Mock<DbContext>();
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", PasswordHash = "hashedpassword" };

            _userRepositoryMock
                .Setup(repo => repo.ValidateUserAsync("testuser", "password"))
                .ReturnsAsync(user);

            // Act
            var result = await _userRepositoryMock.Object.ValidateUserAsync("testuser", "password");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("testuser", result.Name);
        }
        [Fact]
        public async Task ValidateUserAsync_ShouldReturnNull_WhenCredentialsAreIncorrect()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.ValidateUserAsync("testuser", "wrongpassword"))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userRepositoryMock.Object.ValidateUserAsync("testuser", "wrongpassword");

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            _userRepositoryMock
                .Setup(repo => repo.ExistsAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _userRepositoryMock.Object.ExistsAsync(userId);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            int nonExistentUserId = 99;
            _userRepositoryMock
                .Setup(repo => repo.ExistsAsync(nonExistentUserId))
                .ReturnsAsync(false);

            // Act
            var result = await _userRepositoryMock.Object.ExistsAsync(nonExistentUserId);

            // Assert
            Assert.False(result);
        }
        [Fact]
        public async Task AddUserAsync_ShouldHashPasswordAndAddUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "testuser", PasswordHash = "plaintextpassword" };

            _userRepositoryMock
                .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback<User>(u =>
                {
                    // Simula a criptografia da senha
                    u.PasswordHash = PasswordHelper.HashPassword("plaintextpassword");
                });

            // Act
            await _userRepositoryMock.Object.AddUserAsync(user);

            // Assert
            Assert.NotEqual("plaintextpassword", user.PasswordHash); // Garante que a senha foi alterada
            _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task AddUserAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            // Arrange
            var user = new User { Id = 2, Name = "existinguser", PasswordHash = "hashedpassword" };

            _userRepositoryMock
                .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .ThrowsAsync(new InvalidOperationException("O nome de usuário já está em uso."));

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepositoryMock.Object.AddUserAsync(user)
            );

            // Assert
            Assert.Equal("O nome de usuário já está em uso.", exception.Message);
        }
        [Fact]
        public async Task AddUserAsync_ShouldThrowException_WhenDbUpdateFails()
        {
            // Arrange
            var user = new User { Id = 3, Name = "newuser", PasswordHash = "hashedpassword" };

            _userRepositoryMock
                .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .ThrowsAsync(new DbUpdateException("Erro de banco de dados"));

            // Act
            var exception = await Assert.ThrowsAsync<DbUpdateException>(
                () => _userRepositoryMock.Object.AddUserAsync(user)
            );

            // Assert
            Assert.Equal("Erro de banco de dados", exception.Message);
        }
        [Fact]
        public async Task AddAsync_ShouldCallAddMethod()
        {
            // Arrange
            var user = new User { Id = 1, Name = "TestUser", PasswordHash = "hashedpassword" };

            // Act
            await _userRepositoryMock.Object.AddAsync(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(user), Times.Once);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "TestUser", PasswordHash = "hashedpassword" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userRepositoryMock.Object.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnUserList()
        {
            // Arrange
            var users = new List<User> {
                                    new User { Id = 1, Name = "User1", PasswordHash = "hash1" },
                                    new User { Id = 2, Name = "User2", PasswordHash = "hash2" }
                                       };

            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userRepositoryMock.Object.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public void Update_ShouldCallUpdateMethod()
        {
            // Arrange
            var user = new User { Id = 1, Name = "UpdatedUser", PasswordHash = "hashUpdated" };

            // Act
            _userRepositoryMock.Object.Update(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.Update(user), Times.Once);
        }
        [Fact]
        public void Delete_ShouldCallDeleteMethod()
        {
            // Arrange
            var user = new User { Id = 1, Name = "TestUser", PasswordHash = "hashedpassword" };

            // Act
            _userRepositoryMock.Object.Delete(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.Delete(user), Times.Once);
        }
    }
}
