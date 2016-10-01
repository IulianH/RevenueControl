namespace RevenueControl.DomainObjects.Entities
{
    public class TransactionTag
    {
        public int Id { get; set; }

        public string ClientName { get; set; }

        public int TransactionId { get; set; }

        public string Tag { get; set; }
    }
}