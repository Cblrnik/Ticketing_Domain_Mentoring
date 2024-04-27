﻿using System.Net;
using System.Web.Http;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;

namespace Ticketing.Api.Client.Controllers
{
    [Route("/api/venues")]
    public class VenuesController : ApiController
    {
        private readonly Repository<Venue> _venueRepository;

        public VenuesController(Repository<Venue> venueRepository)
        {
            _venueRepository = venueRepository;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetVenues()
        {
            var venues = await _venueRepository.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, venues);
        }

        [HttpGet]
        [Route("{venueId:int}/sections")]
        public async Task<HttpResponseMessage> GetSections(int venueId)
        {
            var sections = (await _venueRepository.GetById(venueId)).Sections;

            return sections != null ? Request.CreateResponse(HttpStatusCode.OK, sections) : Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
