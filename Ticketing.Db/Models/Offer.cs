namespace Ticketing.Db.Models
{
    public class Offer : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public Event? Event { get; set; }

        public IEnumerable<Seat>? Seats { get; set; }

        public IEnumerable<TicketPriceLevel>? PriceLevels { get; set; }
    }
}
