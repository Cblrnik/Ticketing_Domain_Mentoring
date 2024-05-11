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
    public class EventsControllerTests
    {
        public EventsController controller;
        public Mock<Repository<Event>> eventRepositoryMock;
        public Mock<Repository<Offer>> offerRepositoryMock;

        public Event Event;
        public Offer Offer;
        public Seat Seat;

        [SetUp]
        public void Setup()
        {
            Seat = new Seat
            {
                Id = 1,
                Name = "A22",
                SeatStatus = SeatStatus.Available
            };

            Event = new()
            {
                Id = 1,
                Description = "Description",
                StartDate = new DateTime(),
                EndDate = new DateTime(),
                Name = "EventName",
                Venue = new Venue()
                {
                    Sections = new List<Section>
                    {
                        new ()
                        {
                            Id = 1,
                            Seats = new List<Seat>
                            {
                                Seat
                            }
                        }
                    }
                }
            };
            Offer = new()
            {
                Id = 1,
                Event = null,
                Name = string.Empty,
                PriceLevels = new List<TicketPriceLevel>
                {
                    new TicketPriceLevel
                    {
                        Id = 1,
                        Name = string.Empty,
                        Price = 99.90m
                    }
                },
                Seats = new List<Seat>
                {
                    Seat
                }
            };

            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            eventRepositoryMock = new Mock<Repository<Event>>(connectionProviderMock.Object, "Event");
            offerRepositoryMock = new Mock<Repository<Offer>>(connectionProviderMock.Object, "Offer");

            controller = new EventsController(eventRepositoryMock.Object, offerRepositoryMock.Object);

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/events");
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
        public async Task GetEventsAsync_ReturnsAllEvents()
        {
            eventRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Event> { Event });

            var response = await controller.GetEventsAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultStatus = await ReadJsonContentAsync<List<Event>>(response.Content);

            Assert.That(resultStatus[0].Id, Is.EqualTo(1));
            Assert.That(resultStatus[0].Name, Is.EqualTo("EventName"));
        }

        [Test, Order(2)]
        public async Task GetSectionsAsync_GetInfo()
        {
            eventRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(Event);
            offerRepositoryMock.Setup(p => p.GetAllAsync()).ReturnsAsync(new List<Offer> { Offer });

            var response = await controller.GetSectionsAsync(1, 1);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public async Task<T> ReadJsonContentAsync<T>(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
