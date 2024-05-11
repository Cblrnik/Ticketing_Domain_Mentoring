using System.Net;
using System.Web.Http;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;

namespace Ticketing.Api.Client.Controllers
{
    [Route("/api/events")]
    public class EventsController : ApiController
    {
        private readonly Repository<Event> _eventRepository;
        private readonly Repository<Offer> _offerRepository;

        public EventsController(Repository<Event> eventRepository, Repository<Offer> offerRepository)
        {
            _eventRepository = eventRepository;
            _offerRepository = offerRepository;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return Request.CreateResponse(HttpStatusCode.OK, events);
        }

        [HttpGet]
        [Route("{eventId:int}/sections/{sectionId:int}/seats")]
        public async Task<HttpResponseMessage> GetSectionsAsync(int eventId, int sectionId)
        {
            var seats = (await _eventRepository.GetByIdAsync(eventId)).Venue?.Sections?.FirstOrDefault(section => section.Id == sectionId)?.Seats?.ToList();

            var prices = (await _offerRepository.GetAllAsync()).Where(offer =>
                seats != null && seats.Any(seat => 
                    offer.Seats != null && offer.Seats.Contains(seat))).ToList();

            if (seats == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var response = seats.Select(seat => new
            {
                seat.Id,
                seat.Name,
                SectionId = sectionId,
                seat.SeatStatus,
                PriceOptions = prices.FirstOrDefault(price => price.Seats.Contains(seat)).PriceLevels
            });

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
