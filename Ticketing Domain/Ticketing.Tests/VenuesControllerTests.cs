using System.Net;
using Moq;
using Ticketing.Api.Client.Controllers;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.BL.Services;
using Ticketing.Db.Providers;
using Newtonsoft.Json;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Hosting;

namespace Ticketing.Tests
{
    [TestFixture]
    public class VenuesControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetVenuesAsync_ReturnsAllVenues()
        {
            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            var venueRepositoryMock = new Mock<Repository<Venue>>(connectionProviderMock.Object, "Venue");

            var controller = new VenuesController(venueRepositoryMock.Object);

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/venues");
            var route = config.Routes.MapHttpRoute("default", "");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "GetVenuesAsync" } });
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            var venues = new List<Venue>
            {
                new Venue
                {
                    Id = 1,
                    Name = "Minsk Arena",
                    Address = "Minsk",
                    Sections = null
                },
                new Venue
                {
                    Id = 2,
                    Name = "Almaty Arena",
                    Address = "Almaty",
                    Sections = null
                },
            };

            venueRepositoryMock.Setup(o => o.GetAllAsync()).ReturnsAsync(venues);

            var response = await controller.GetVenuesAsync();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultVenues = await ReadJsonContentAsync<List<Venue>>(response.Content);

            Assert.That(resultVenues, Is.Not.Null);
            Assert.That(resultVenues.Count(), Is.EqualTo(2));
            Assert.That(resultVenues[0].Address, Is.EqualTo(venues[0].Address));
            Assert.That(resultVenues[1].Address, Is.EqualTo(venues[1].Address));
        }

        [Test]
        public async Task GetVenuesAsync_ReturnsAllSectionsForVenue()
        {
            var connectionProviderMock = new Mock<IConnectionStringProvider>();
            var venueRepositoryMock = new Mock<Repository<Venue>>(connectionProviderMock.Object, "Venue");

            var controller = new VenuesController(venueRepositoryMock.Object);

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/venues");
            var route = config.Routes.MapHttpRoute("default", "{venueId:int}/sections");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "GetSectionsAsync" } });
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);

            var venue = new Venue
            {
                Id = 1,
                Name = "Minsk Arena",
                Address = "Minsk",
                Sections = new List<Section>
                {
                    new Section
                    {
                        Id = 1,
                        Name = "A1",
                        Seats = null
                    },
                    new Section
                    {
                        Id = 2,
                        Name = "A2",
                        Seats = null
                    }
                }
            };

            venueRepositoryMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(venue);


            var response = await controller.GetSectionsAsync(venue.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var resultVenues = await ReadJsonContentAsync<List<Section>>(response.Content);

            Assert.That(resultVenues, Is.Not.Null);
            Assert.That(resultVenues.Count(), Is.EqualTo(2));
            Assert.That(resultVenues[0].Name, Is.EqualTo(venue.Sections.First().Name));
            Assert.That(resultVenues[1].Name, Is.EqualTo(venue.Sections.Last().Name));
        }

        public async Task<T> ReadJsonContentAsync<T>(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}