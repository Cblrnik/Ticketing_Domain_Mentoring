namespace Ticketing.Db.Models
{
    public class Payment : IIdentity
    {
        public int Id { get; set; }

        public PaymentStatus Status { get; set; }

        public double Amount { get; set; }
    }
}
