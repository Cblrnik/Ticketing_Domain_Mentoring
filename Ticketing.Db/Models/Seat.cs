namespace Ticketing.Db.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public Offer? Offer { get; set; }
    }
}
