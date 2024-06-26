﻿using Ticketing.Db.DAL;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.BL.Services
{
    public class OrderService
    {
        private readonly Repository<Seat> _seatRepository;
        private readonly Repository<Offer> _offerRepository;
        private readonly Repository<TicketPriceLevel> _ticketPriceLevelRepository;
        private readonly CartProvider _cartProvider;

        public OrderService(Repository<Seat> seatRepository, Repository<Offer> offerRepository, Repository<TicketPriceLevel> ticketPriceLevelRepository, CartProvider cartProvider)
        {
            _seatRepository = seatRepository;
            _offerRepository = offerRepository;
            _ticketPriceLevelRepository = ticketPriceLevelRepository;
            _cartProvider = cartProvider;
        }

        public Cart GetById(Guid id)
        {
            return _cartProvider.Carts.FirstOrDefault(cart => cart.Id == id)!;
        }

        public async Task<Cart> AddToCartAsync(Guid cartId, CartDetails details)
        {
            var offer = await GetOfferAsync(details.EventId, details.SeatId);
            var cart = GetById(cartId);
            cart.OrderDetails.Add(new OrderDetails
            {
                OfferId = offer!.Id,
                PriceId = offer.PriceLevels!.FirstOrDefault(level => level.Id == details.PriceId)!.Id,
                SeatId = details.SeatId
            });

            await CalculateAmountAsync(cart);

            return cart;
        }

        private async Task<Offer> GetOfferAsync(int eventId, int seatId)
        {
            return (await _offerRepository.GetAllAsync()).ToList().FirstOrDefault(offer => offer.Event?.Id == eventId && offer.Seats!.Any(item => item.Id == seatId))!;
        }

        public async Task DeleteSeat(Guid cartId, int eventId, int seatId)
        {
            var cart = GetById(cartId);
            var offer = await GetOfferAsync(eventId, seatId);

            var index = cart.OrderDetails.FindIndex(details => details.SeatId == seatId && details.OfferId == offer.Id);

            cart.OrderDetails.RemoveAt(index);
        }

        public async Task BookSeatsAsync(Guid cartId)
        {
            var cart = GetById(cartId);

            var seatIds = cart.OrderDetails.Select(details => details.SeatId);

            var seats = (await _seatRepository.GetAllAsync()).Where(seat => seatIds.Contains(seat.Id));

            foreach (var seat in seats)
            {
                seat.SeatStatus = SeatStatus.Booked;

                await _seatRepository.UpdateAsync(seat);
            }
        }

        private async Task CalculateAmountAsync(Cart cart)
        {
            var amount = 0m;
            foreach (var detail in cart.OrderDetails)
            {
                var price = await _ticketPriceLevelRepository.GetByIdAsync(detail.PriceId);
                amount += price.Price;
            }

            cart.Amount = amount;
        }
    }
}
