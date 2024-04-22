namespace Ticketing.Db.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public Customer? Customer { get; set; }

        public Event? Event { get; set; }

        public Seat? Seat { get; set; }

        public Section? Section { get; set; }

        public int Status { get; set; }
    }
}
