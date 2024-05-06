using Moq;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.BL.Services;
using Ticketing.Db.Providers;

namespace Ticketing.Tests
{
    [TestFixture]
    public class PaymentsServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task UpdatePaymentStatusAsync_StatusesWereUpdated()
        {
            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            var paymentRepositoryMock = new Mock<Repository<Payment>>(connectionProviderMock.Object, "Payment");
            var orderRepositoryMock = new Mock<Repository<Order>>(connectionProviderMock.Object, "Order");
            var seatRepositoryMock = new Mock<Repository<Seat>>(connectionProviderMock.Object, "Seat");
            var service = new PaymentsService(paymentRepositoryMock.Object, orderRepositoryMock.Object, seatRepositoryMock.Object);

            var payment = new Payment
            {
                Id = 1,
                Amount = 990.9,
                Status = PaymentStatus.InProgress
            };

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

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Event = null,
                    Id = 1,
                    Seat = seats[0],
                    Status = 0
                },
                new Ticket
                {
                    Event = null,
                    Id = 2,
                    Seat = seats[1],
                    Status = 0
                }
            };

            var order = new Order()
            {
                Customer = null,
                Id = 1,
                Payment = payment,
                Tickets = tickets
            };



            paymentRepositoryMock.Setup(p => p.GetById(1)).Returns(Task.FromResult(payment));
            orderRepositoryMock.Setup(o => o.GetAll()).Returns(Task.FromResult((ICollection<Order>)new List<Order>{ order }));
            seatRepositoryMock.Setup(s => s.Update(seats[0])).Returns(Task.Run(() => { }));
            seatRepositoryMock.Setup(s => s.Update(seats[1])).Returns(Task.Run(() => { }));

            await service.UpdatePaymentStatusAsync(1, PaymentStatus.Success, SeatStatus.Sold);

            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Success));
            foreach (var seat in seats)
            {
                Assert.That(seat.SeatStatus, Is.EqualTo(SeatStatus.Sold));
            }
        }


    }
}