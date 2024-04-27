using System.Net;
using System.Web.Http;
using Ticketing.BL.Services;
using Ticketing.Db.Models;

namespace Ticketing.Api.Client.Controllers
{
    [Route("/api/orders")]
    public class OrdersController : ApiController
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Route("carts/{cartId:Guid}")]
        public HttpResponseMessage GetCart(Guid cartId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _orderService.GetById(cartId));
        }

        [HttpPost]
        [Route("carts/{cartId:Guid}")]
        public async Task<HttpResponseMessage> AddToCart([FromUri] Guid cartId, [FromBody] CartDetails details)
        {
            var cart = await _orderService.AddToCart(cartId, details);

            return Request.CreateResponse(HttpStatusCode.OK, cart);
        }

        [HttpDelete]
        [Route("carts/{cartId:Guid}/events/{eventId:int}/seats/{seatId:int}")]
        public async Task<HttpResponseMessage> DeleteSeatFromCart([FromUri] Guid cartId, [FromUri] int eventId, [FromUri] int seatId)
        {
            await _orderService.DeleteSeat(cartId, eventId, seatId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        [Route("carts/{cartId:Guid}/book")]
        public async Task<HttpResponseMessage> BookSeats([FromUri] Guid cartId)
        {
            await _orderService.BookSeats(cartId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
