using Core;
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
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            // Criptografa a senha antes de adicionar o usuário
            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);
            try
            {
                // Chama o método AddAsync sobrescrito
                await AddAsync(user);

                // Salva as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Verifica se o erro foi causado por uma violação de UNIQUE CONSTRAINT
                if (ex.InnerException?.Message.Contains("unique") == true)
                {
                    throw new InvalidOperationException("O nome de usuário já está em uso.");
                }
                // Se for outro erro, relança a exceção original
                throw;
            }
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            // Busca o usuário pelo nome de usuário
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == username);

            if (user == null)
            {
                return null; // Usuário não encontrado
            }

            // Verifica se a senha fornecida corresponde ao hash armazenado
            bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.PasswordHash);

            return isPasswordValid ? user : null; // Retorna o usuário se a senha for válida
        }
        public async Task<bool> ExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
