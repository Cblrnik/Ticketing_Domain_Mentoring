namespace Ticketing.Db.Models
{
    public class Manager : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }
    }
}
