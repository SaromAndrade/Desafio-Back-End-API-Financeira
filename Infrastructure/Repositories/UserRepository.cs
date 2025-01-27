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

            // Chama o método AddAsync sobrescrito
            await AddAsync(user);

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();
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
    }
}
