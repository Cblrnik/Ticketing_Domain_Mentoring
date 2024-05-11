using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Http;
using Ticketing.Api.Client.Controllers;
using Ticketing.BL.Services;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Tests
{
    [TestFixture]
    public class PaymentsControllerTests
    {
        public PaymentsController controller;
        public Mock<Repository<Payment>> paymentRepositoryMock;
        public Mock<Repository<Order>> orderRepositoryMock;
        public Mock<Repository<Seat>> seatRepositoryMock;
        public PaymentsService paymentService;

        public Payment Payment = new()
        {
            Id = 1,
            Amount = 99.90,
            Status = PaymentStatus.InProgress
        };

        public Order Order;
        public Seat Seat;

        [SetUp]
        public void Setup()
        {
            Seat = new Seat()
            {
                Id = 1,
                Name = "A22",
                SeatStatus = SeatStatus.Available
            };
            Order = new Order()
            {
                Id = 1,
                Customer = null,
                Payment = Payment,
                Tickets = new List<Ticket>() { new ()
                {
                    Id = 1,
                    Event = null,
                    Seat = Seat,
                    Status = 2
                }}
            };
            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            paymentRepositoryMock = new Mock<Repository<Payment>>(connectionProviderMock.Object, "Payment");
            orderRepositoryMock = new Mock<Repository<Order>>(connectionProviderMock.Object, "Order");
            seatRepositoryMock = new Mock<Repository<Seat>>(connectionProviderMock.Object, "Seat");
            paymentService = new PaymentsService(paymentRepositoryMock.Object, orderRepositoryMock.Object, seatRepositoryMock.Object);

            controller = new PaymentsController(paymentRepositoryMock.Object, paymentService);

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/payments");
            var route = config.Routes.MapHttpRoute("default", "");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "GetVenuesAsync" } });
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
        }

        [TearDown]
        public void Dispose()
        {
            controller.Dispose();
        }

        [Test, Order(1)]
        public async Task GetStatus_ReturnsPaymentStatus()
        {
            paymentRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(Payment);

            var response = controller.GetStatus(1);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultStatus = await ReadJsonContentAsync<PaymentStatus>(response.Content);

            Assert.That(resultStatus, Is.EqualTo(PaymentStatus.InProgress));
        }

        [Test, Order(2)]
        public async Task CompletePaymentAsync_UpdatesPaymentStatusToSuccess()
        {
            paymentRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(Payment);
            paymentRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Payment>())).Callback<Payment>((updatedPayment) =>
            {
                Payment = updatedPayment;
            }).Returns(Task.CompletedTask);

            orderRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Order> { Order });
            seatRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Seat>())).Callback<Seat>((updatedSeat) =>
            {
                Seat = updatedSeat;
            }).Returns(Task.CompletedTask);

            var response = await controller.CompletePaymentAsync(1);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(Payment.Status, Is.EqualTo(PaymentStatus.Success));
        }

        [Test, Order(3)]
        public async Task CompletePaymentAsync_UpdatesPaymentStatusToFailed()
        {
            paymentRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(Payment);
            paymentRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Payment>())).Callback<Payment>((updatedPayment) =>
            {
                Payment = updatedPayment;
            }).Returns(Task.CompletedTask);

            orderRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Order> { Order });
            seatRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Seat>())).Callback<Seat>((updatedSeat) =>
            {
                Seat = updatedSeat;
            }).Returns(Task.CompletedTask);

            var response = await controller.FailPaymentAsync(1);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(Payment.Status, Is.EqualTo(PaymentStatus.Failed));
        }

        public async Task<T> ReadJsonContentAsync<T>(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
