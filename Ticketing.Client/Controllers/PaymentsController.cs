using System.Net;
using System.Web.Http;
using Ticketing.BL.Services;
using Ticketing.Db.DAL;
using Ticketing.Db.Models;

namespace Ticketing.Api.Client.Controllers
{
    [Route("/api/payments")]
    public class PaymentsController : ApiController
    {
        private readonly Repository<Payment> _paymentRepository;
        private readonly PaymentsService _paymentsService;

        public PaymentsController(Repository<Payment> paymentRepository, PaymentsService paymentsService)
        {
            _paymentRepository = paymentRepository;
            _paymentsService = paymentsService;
        }

        [HttpGet]
        [Route("{paymentId:int}")]
        public HttpResponseMessage GetStatus(int paymentId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _paymentRepository.GetByIdAsync(paymentId).Result.Status);
        }

        [HttpPost]
        [Route("{paymentId}/complete")]
        public async Task<HttpResponseMessage> CompletePaymentAsync(int paymentId)
        {
            await _paymentsService.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Success, SeatStatus.Sold);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("{paymentId}/failed")]
        public async Task<HttpResponseMessage> FailPaymentAsync(int paymentId)
        {
            await _paymentsService.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Failed, SeatStatus.Available);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
