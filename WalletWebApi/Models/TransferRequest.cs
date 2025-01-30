namespace WalletWebApi.Models
{
    public class TransferRequest
    {
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
    }
}
