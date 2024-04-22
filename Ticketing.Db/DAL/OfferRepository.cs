using System;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class OfferRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Offer";

        public OfferRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Offer> GetAllOffers()
        {
            const string sql = $"SELECT * FROM {TableName}";
            var offers = _dataAccess.Query<Offer>(sql).ToList();

            foreach (var offer in offers)
            {
                offer.PriceLevels = GetPriceLevelsForOffer(offer.Id);
            }

            return offers;
        }

        public Offer GetOfferById(int offerId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @OfferId";
            var offer = _dataAccess.QueryFirstOrDefault<Offer>(sql, new { OfferId = offerId });

            offer.PriceLevels = GetPriceLevelsForOffer(offer.Id);

            return offer;
        }

        public void AddOffer(Offer offer)
        {
            const string sql = $"INSERT INTO {TableName} (Name, Price) VALUES (@Name, @Price)";
            _dataAccess.Execute(sql, offer);

            if (offer.PriceLevels == null || !offer.PriceLevels.Any()) return;
            foreach (var priceLevel in offer.PriceLevels)
            {
                AddPriceLevelForOffer(offer.Id, priceLevel);
            }
        }

        public void UpdateOffer(Offer offer)
        {
            const string sql = $"UPDATE {TableName} SET Name = @Name, Price = @Price WHERE Id = @Id";
            _dataAccess.Execute(sql, offer);

            if (offer.PriceLevels == null || !offer.PriceLevels.Any()) return;

            const string deleteSql = "DELETE FROM OfferPriceLevels WHERE OfferId = @OfferId";
            _dataAccess.Execute(deleteSql, new { OfferId = offer.Id });

            foreach (var priceLevel in offer.PriceLevels)
            {
                AddPriceLevelForOffer(offer.Id, priceLevel);
            }
        }

        public void DeleteOffer(int offerId)
        {
            const string deleteOfferSql = $"DELETE FROM {TableName} WHERE Id = @OfferId";
            _dataAccess.Execute(deleteOfferSql, new { OfferId = offerId });

            const string deleteSql = "DELETE FROM OfferPriceLevels WHERE OfferId = @OfferId";
            _dataAccess.Execute(deleteSql, new { OfferId = offerId });
        }

        private IEnumerable<TicketPriceLevel> GetPriceLevelsForOffer(int offerId)
        {
            const string sql = "SELECT * FROM TicketPriceLevel WHERE Id IN (SELECT PriceLevelId FROM OfferPriceLevels WHERE OfferId = @OfferId)";
            return _dataAccess.Query<TicketPriceLevel>(sql, new { OfferId = offerId });
        }

        private void AddPriceLevelForOffer(int offerId, TicketPriceLevel priceLevel)
        {
            const string sql = "INSERT INTO OfferPriceLevels (OfferId, PriceLevelId) VALUES (@OfferId, @PriceLevelId)";
            _dataAccess.Execute(sql, new { OfferId = offerId, PriceLevelId = priceLevel.Id });
        }
    }
}
