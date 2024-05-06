namespace Ticketing.Db.Models
{
    public class Section : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public IEnumerable<Seat>? Seats { get; set; }
    }
}
