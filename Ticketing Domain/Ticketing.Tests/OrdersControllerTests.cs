using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using Newtonsoft.Json;

namespace Ticketing.Tests
{
    [TestFixture]
    public class OrdersControllerTests
    {
        public OrderService orderService;
        public OrdersController controller;

        public Mock<Repository<Seat>> seatRepositoryMock;
        public Mock<Repository<Offer>> offerRepositoryMock;
        public Mock<Repository<TicketPriceLevel>> ticketPriceLevelRepositoryMock;
        public CartProvider cartProvider;

        public List<TicketPriceLevel> ticketPriceLevelList = new List<TicketPriceLevel>
        {
            new ()
            {
                Id = 1,
                Name = string.Empty,
                Price = 89.90m
            },
            new ()
            {
                Id = 2,
                Name = string.Empty,
                Price = 89.90m
            },
        };

        public Offer Offer;

        public Seat Seat;

        [SetUp]
        public void Setup()
        {
            Seat = new()
            {
                Id = 1,
                Name = string.Empty,
                SeatStatus = SeatStatus.Available
            };

            Offer = new Offer
            {
                Event = new Event()
                {
                    Id = 1
                },
                Id = 1,
                Name = string.Empty,
                PriceLevels = ticketPriceLevelList,
                Seats = new List<Seat>
                {
                    Seat
                }
            };

            var connectionProviderMock = new Mock<IConnectionStringProvider>();

            seatRepositoryMock = new Mock<Repository<Seat>>(connectionProviderMock.Object, "Seat");
            offerRepositoryMock = new Mock<Repository<Offer>>(connectionProviderMock.Object, "Offer");
            ticketPriceLevelRepositoryMock = new Mock<Repository<TicketPriceLevel>>(connectionProviderMock.Object, "TicketPriceLevel");
            cartProvider = new CartProvider();

            orderService = new OrderService(seatRepositoryMock.Object, offerRepositoryMock.Object, ticketPriceLevelRepositoryMock.Object, cartProvider);
            controller = new OrdersController(orderService);

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/orders");
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
        public async Task GetCart_ReturnsCart()
        {
            var guidToRequest = cartProvider.Carts.FirstOrDefault().Id;
            var response = controller.GetCart(guidToRequest);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultStatus = await ReadJsonContentAsync<Cart>(response.Content);

            Assert.That(resultStatus.Id, Is.EqualTo(guidToRequest));
            Assert.That(resultStatus.Amount, Is.EqualTo(0));
        }

        [Test, Order(2)]
        public async Task AddToCartAsync_ReturnsCart()
        {
            var guidToRequest = cartProvider.Carts.FirstOrDefault().Id;
            var cartDetails = new CartDetails()
            {
                EventId = 1,
                PriceId = 1,
                SeatId = 1
            };

            offerRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Offer> { Offer });
            ticketPriceLevelRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ticketPriceLevelList[0]);

            var response = await controller.AddToCartAsync(guidToRequest, cartDetails);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultStatus = await ReadJsonContentAsync<Cart>(response.Content);

            Assert.That(resultStatus.Id, Is.EqualTo(guidToRequest));
            Assert.That(resultStatus.Amount, Is.EqualTo(89.90));
        }

        [Test, Order(3)]
        public async Task BookSeats_SeatWasBooked()
        {
            var guidToRequest = cartProvider.Carts.FirstOrDefault();

            var cartDetails = new CartDetails()
            {
                EventId = 1,
                PriceId = 1,
                SeatId = 1
            };

            offerRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Offer> { Offer });
            ticketPriceLevelRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ticketPriceLevelList[0]);
            seatRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Seat> { Seat });
            seatRepositoryMock.Setup(p => p.UpdateAsync(It.IsAny<Seat>())).Callback<Seat>((updatedSeat) =>
            {
                Seat = updatedSeat;
            }).Returns(Task.CompletedTask);

            var addResponse = await controller.AddToCartAsync(guidToRequest.Id, cartDetails);
            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var response = await controller.BookSeats(guidToRequest.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, Order(4)]
        public async Task DeleteSeatFromCartAsync_DeletesSeat()
        {
            var guidToRequest = cartProvider.Carts.FirstOrDefault();
            var cartDetails = new CartDetails()
            {
                EventId = 1,
                PriceId = 1,
                SeatId = 1
            };

            offerRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Offer> { Offer });
            ticketPriceLevelRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ticketPriceLevelList[0]);

            var addResponse = await controller.AddToCartAsync(guidToRequest.Id, cartDetails);
            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var response = await controller.DeleteSeatFromCartAsync(guidToRequest.Id, 1, 1);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task<T> ReadJsonContentAsync<T>(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
