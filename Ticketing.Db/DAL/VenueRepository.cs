using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class VenueRepository : Repository<Venue>
    {
        private readonly Repository<Section> _sectionRepository;

        public VenueRepository(IConnectionStringProvider connectionStringProvider, Repository<Section> sectionRepository) : base(connectionStringProvider, "Venue")
        {
            _sectionRepository = sectionRepository;
        }

        public override async Task<ICollection<Venue>> GetAll()
        {
            await base.GetAll();

            foreach (var venue in Entities)
            {
                venue.Sections = await GetSections(venue.Id).ConfigureAwait(false);
            }

            return Entities;
        }

        public override async Task<int> Create(Venue entity)
        {
            var sql = $"INSERT INTO [{TableName}] (Name, Address) VALUES (@Name, @Address)";
            RefreshCache();
            entity.Id = await ExecuteAsync(sql, entity);

            if (entity.Sections != null) await CreateSections(entity.Sections, entity.Id);

            return entity.Id;
        }

        public override async Task Update(Venue entity)
        {
            var sql = $"UPDATE [{TableName}] SET Name = @Name, Address = @Address WHERE Id = @Id";
            await ExecuteAsync(sql, entity);
            RefreshCache();

            if (entity.Sections != null) await UpdateSectionsVenue(entity.Sections, entity.Id);
        }

        public override async Task Delete(int id)
        {
            var sql = $"DELETE FROM [{TableName}] WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
            RefreshCache();

            await DeleteSections(id);
        }

        private async Task<ICollection<Section>> GetSections(int id)
        {
            var sql = "SELECT * FROM Section WHERE VenueId = @Id";
            return await QueryAsync<Section>(sql, new { Id = id });
        }

        public async Task CreateSections(IEnumerable<Section> sections, int venueId)
        {
            foreach (var section in sections)
            {
                await _sectionRepository.Create(section);
            }

            await UpdateSectionsVenue(sections, venueId);
        }

        public async Task UpdateSectionsVenue(IEnumerable<Section> sections, int venueId)
        {
            foreach (var section in sections)
            {
                var sql = "UPDATE Section SET VenueId = @VenueId WHERE Id = @Id";
                await ExecuteAsync(sql, new { VenueId = venueId, section.Id });
            }
        }

        private async Task DeleteSections(int id)
        {
            var sql = "SELECT Id FROM [Section] WHERE VenueId = @Id";

            var ids = await QueryAsync<int>(sql, new { Id = id });
            foreach (var sectionId in ids)
            {
                await _sectionRepository.Delete(sectionId);
            }
        }
    }
}
