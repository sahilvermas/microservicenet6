using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ordering.Application.Models;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentException(null, nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentException(null, nameof(mapper));
            _emailService = emailService ?? throw new ArgumentException(null, nameof(emailService));
            _logger = logger ?? throw new ArgumentException(null, nameof(logger));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepository.AddAsync(orderEntity);
            _logger.LogInformation($"Order {newOrder.Id} is successfully created.");
            await SendMail(newOrder);
            return newOrder.Id;
        }

        private async Task SendMail(EntityBase order)
        {
            var email = new Email()
            {
                To = "www.sahilv@gmail.com",
                Body = $"Order was created.",
                Subject = "Order was created"
            };

            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
            }
        }
    }
}
