using System.ComponentModel.DataAnnotations;

namespace WalletWebApi.Models
{
    public class TransferFilterRequest
    {
        [Required]
        public int UserId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
