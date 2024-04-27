namespace Ticketing.Db.Models
{
    public class Ticket : IIdentity
    {
        public int Id { get; set; }

        public Event? Event { get; set; }

        public Seat? Seat { get; set; }

        public int Status { get; set; }
    }
}
