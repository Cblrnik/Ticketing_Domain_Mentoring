﻿namespace Ticketing.Db.Models
{
    public class Venue : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public IEnumerable<Section>? Sections { get; set; }
    }
}
