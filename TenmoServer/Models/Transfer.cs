namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }

        public int TransferTypeId { get; set; }

        public string TransferTypeDesc { get; set; }

        public int TransferStatusId { get; set; }

        public string TransferStatusDesc { get; set; }

        public int AccountFrom { get; set; }

        public string UserFrom { get; set; }

        public int AccountTo { get; set; }

        public string UserTo { get; set; }

        public decimal Amount { get; set; }
    }
}
