using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        // Valida a senha do usuário
        Task<User?> ValidateUserAsync(string username, string password);
        Task AddUserAsync(User user);
        Task<bool> ExistsAsync(int userId);
    }
}
