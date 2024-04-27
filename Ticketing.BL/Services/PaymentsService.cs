﻿using Ticketing.Db.DAL;
using Ticketing.Db.Models;

namespace Ticketing.BL.Services
{
    public class PaymentsService
    {
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<Seat> _seatRepository;
        private readonly Repository<Payment> _paymentRepository;

        public PaymentsService(Repository<Payment> paymentRepository, Repository<Order> orderRepository, Repository<Seat> seatRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _seatRepository = seatRepository;
        }

        public async Task UpdatePaymentStatus(int paymentId, PaymentStatus status, SeatStatus seatStatus)
        {
            var payment = await _paymentRepository.GetById(paymentId);
            payment.Status = status;
            await _paymentRepository.Update(payment);

            var tickets = (await _orderRepository.GetAll().ConfigureAwait(false)).FirstOrDefault(order => order.Payment?.Id == paymentId)?.Tickets?.ToList();

            if (tickets != null)
            {
                var seats = tickets.Select(ticket => ticket.Seat);
                foreach (var seat in seats)
                {
                    seat.SeatStatus = seatStatus;
                    await _seatRepository.Update(seat);
                }
            }
        }
    }
}
