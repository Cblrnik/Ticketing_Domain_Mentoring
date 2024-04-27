namespace Ticketing.Db.Models
{
    public class Customer : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }
    }
}
