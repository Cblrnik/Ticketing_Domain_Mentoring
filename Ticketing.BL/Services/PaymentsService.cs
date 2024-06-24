using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.Notification.Models;
using Ticketing.Notification.Services;

namespace Ticketing.BL.Services
{
    public class PaymentsService
    {
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<Seat> _seatRepository;
        private readonly Repository<Payment> _paymentRepository;
        private readonly KafkaProducer _producer;

        public PaymentsService(Repository<Payment> paymentRepository, Repository<Order> orderRepository, Repository<Seat> seatRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _seatRepository = seatRepository;
            _producer = new KafkaProducer();
        }

        public async Task UpdatePaymentStatusAsync(int paymentId, PaymentStatus status, SeatStatus seatStatus)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            payment.Status = status;
            await _paymentRepository.UpdateAsync(payment);

            var order = (await _orderRepository.GetAllAsync().ConfigureAwait(false)).FirstOrDefault(order => order.Payment?.Id == paymentId);

            var tickets = order?.Tickets?.ToList();

            if (tickets != null)
            {
                var seats = tickets.Select(ticket => ticket.Seat);
                foreach (var seat in seats)
                {
                    seat.SeatStatus = seatStatus;
                    await _seatRepository.UpdateAsync(seat);
                }
            }

            if (status == PaymentStatus.Success)
            {
                var message = new NotificationMessage
                {
                    OperationName = "sold",
                    Timestamp = DateTime.UtcNow,
                    NotificationParameters = new NotificationParameters
                    {
                        CustomerEmail = order?.Customer?.Email,
                        CustomerName = order?.Customer?.Name
                    },
                    NotificationContent = new NotificationContent
                    {
                        OrderAmount = payment.Amount,
                        OrderSummary = $"{tickets?.Count} tickets for the event"
                    }
                };

                await _producer.SendMessageAsync("ProcessingTask", message);
            }
        }
    }
}
