using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Verifica se já existem usuários no banco de dados
            if (context.Users.Any())
            {
                return; // O banco de dados já foi inicializado
            }
            // Adiciona usuários iniciais
            var users = new User[]
            {
                new User
                    {
                        Name = "João Silva",
                        PasswordHash = "hashedpassword123" // Use um hash real em produção
                    },
                new User
                    {
                        Name = "Maria Oliveira",
                        PasswordHash = "hashedpassword456" // Use um hash real em produção
                    }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
            // Adiciona carteiras iniciais
            var wallets = new Wallet[]
                {
                    new Wallet
                        {
                            Balance = 1000.00m,
                            UserId = users[0].Id // Relacionado ao usuário João Silva
                        },
                    new Wallet
                        {
                            Balance = 500.00m,
                            UserId = users[1].Id // Relacionado ao usuário Maria Oliveira
                        }
                };
            context.Wallets.AddRange(wallets);
            context.SaveChanges();
        }
    }
}
