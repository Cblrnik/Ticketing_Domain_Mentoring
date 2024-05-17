using Moq;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.BL.Services;
using Ticketing.Db.Providers;

namespace Ticketing.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddToCartAsync_ReturnsCartWithValidAmount()
        {
            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            var seatRepositoryMock = new Mock<Repository<Seat>>(connectionProviderMock.Object, "Seat");
            var offerRepositoryMock = new Mock<Repository<Offer>>(connectionProviderMock.Object, "Offer");
            var ticketPriceLevelRepositoryMock = new Mock<Repository<TicketPriceLevel>>(connectionProviderMock.Object, "TicketPriceLevel");
            var cartProvider = new CartProvider();

            var service = new OrderService(seatRepositoryMock.Object, offerRepositoryMock.Object, ticketPriceLevelRepositoryMock.Object, cartProvider);

            var seats = new List<Seat>
            {
                new Seat
                {
                    Id = 1,
                    Name = "A12|1",
                    SeatStatus = SeatStatus.Booked
                },
                new Seat
                {
                    Id = 2,
                    Name = "A12|2",
                    SeatStatus = SeatStatus.Booked
                }
            };

            var ticketPriceLevels = new List<TicketPriceLevel>
            {
                new TicketPriceLevel
                {
                    Id = 1,
                    Name = "Adult",
                    Price = 70
                },
                new TicketPriceLevel
                {
                    Id = 2,
                    Name = "Kid",
                    Price = 50
                },
            };

            var offer = new Offer
            {
                Id = 1,
                Event = new Event {Id = 1},
                Name = "A21|1",
                PriceLevels = ticketPriceLevels,
                Seats = seats
            };

            ticketPriceLevelRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ticketPriceLevels[0]);
            ticketPriceLevelRepositoryMock.Setup(p => p.GetByIdAsync(2)).ReturnsAsync(ticketPriceLevels[1]);
            offerRepositoryMock.Setup(o => o.GetAllAsync()).ReturnsAsync(new List<Offer> { offer });

            var cart = await service.AddToCartAsync(cartProvider.Carts[0].Id, new CartDetails { EventId = 1, PriceId = 1, SeatId = 1});

            Assert.That(cart.Amount, Is.EqualTo(70));

            var updatedCart = await service.AddToCartAsync(cartProvider.Carts[0].Id, new CartDetails { EventId = 1, PriceId = 2, SeatId = 2 });
            Assert.That(cart.Amount, Is.EqualTo(120));
        }
    }
}