namespace WalletWebApi.Models
{
    public class DepositRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
