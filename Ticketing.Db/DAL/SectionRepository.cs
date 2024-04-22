using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class SectionRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Section";
        private readonly SeatRepository _seatRepository;

        public SectionRepository(IConnectionStringProvider connectionStringProvider, SeatRepository seatRepository)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
            _seatRepository = seatRepository;
        }

        public IEnumerable<Section> GetAllSections()
        {
            const string sql = $"SELECT * FROM {TableName}";
            var sections = _dataAccess.Query<Section>(sql).ToList();

            foreach (var section in sections)
            {
                section.Seats = GetSeatsForSection(section.Id);
            }

            return sections;
        }

        public Section GetSectionById(int sectionId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @SectionId";
            var section = _dataAccess.QueryFirstOrDefault<Section>(sql, new { SectionId = sectionId });

            section.Seats = GetSeatsForSection(section.Id);

            return section;
        }

        public void AddSection(Section section)
        {
            const string sql = $"INSERT INTO {TableName} (Name) VALUES (@Name)";
            _dataAccess.Execute(sql, section);

            if (section.Seats == null || !section.Seats.Any()) return;
            foreach (var seat in section.Seats)
            {
                _seatRepository.AddSeat(seat);
            }
        }

        public void UpdateSection(Section section)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name WHERE Id = @Id";
            _dataAccess.Execute(sql, section);
        }

        public void DeleteSection(int sectionId)
        {
            const string deleteSectionSql = $"DELETE FROM {TableName} WHERE Id = @SectionId";
            _dataAccess.Execute(deleteSectionSql, new { SectionId = sectionId });
        }

        private IEnumerable<Seat> GetSeatsForSection(int sectionId)
        {
            const string sql = "SELECT * FROM Seats WHERE SectionId = @SectionId";
            return _dataAccess.Query<Seat>(sql, new { SectionId = sectionId });
        }
    }
}
