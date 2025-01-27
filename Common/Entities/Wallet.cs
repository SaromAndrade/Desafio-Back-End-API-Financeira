using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Core.Entities
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Balance { get; set; } = 0; // Saldo inicial

        [Required]
        public int UserId { get; set; } // Chave estrangeira para o usuário

        public User User { get; set; } // Relacionamento 1:1 com o usuário

        public ICollection<Transaction> Transactions { get; set; } // Relacionamento 1:N com transações
    }
}
