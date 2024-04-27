using System.Net;
using System.Web.Http;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;

namespace Ticketing.Api.Client.Controllers
{
    [Route("/api/venues")]
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
        public async Task<HttpResponseMessage> GetEvents()
        {
            var events = await _eventRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, events);
        }

        [HttpGet]
        [Route("{eventId:int}/sections/{sectionId:int}/seats")]
        public async Task<HttpResponseMessage> GetSections(int eventId, int sectionId)
        {
            var seats = (await _eventRepository.GetById(eventId)).Venue?.Sections?.FirstOrDefault(section => section.Id == sectionId)?.Seats?.ToList();



            var prices = (await _offerRepository.GetAll()).Where(offer =>
                seats != null && seats.Any(seat => 
                    offer.Seats != null && offer.Seats.Contains(seat))).ToList();


            if (seats == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
            {
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
}
