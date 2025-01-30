using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; } // Valor da transação

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow; // Data da transação

        [Required]
        public int WalletId { get; set; } // Chave estrangeira para a carteira
        [JsonIgnore]
        public Wallet Wallet { get; set; } // Relacionamento N:1 com a carteira

        public int? SenderUserId { get; set; } // Usuário que enviou a transferência (opcional)
        [JsonIgnore]
        public User SenderUser { get; set; } // Relacionamento N:1 com o usuário remetente

        public int? ReceiverUserId { get; set; } // Usuário que recebeu a transferência (opcional)

        [JsonIgnore] 
        public User ReceiverUser { get; set; } // Relacionamento N:1 com o usuário destinatário

        [Required]
        public TransactionType Type { get; set; } // Tipo de transação (enum)
    }
    public enum TransactionType
    {
        Deposit,    // Adição de saldo
        Transfer    // Transferência entre usuários
    }
}
