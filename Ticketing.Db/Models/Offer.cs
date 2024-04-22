namespace Ticketing.Db.Models
{
    public class Offer
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public IEnumerable<TicketPriceLevel>? PriceLevels { get; set; }
    }
}
