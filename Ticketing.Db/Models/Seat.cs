namespace Ticketing.Db.Models
{
    public class Seat : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public SeatStatus SeatStatus { get; set; }
    }
}
