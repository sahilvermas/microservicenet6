using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;
        private readonly IDiscountRepository _repository;

        public DiscountService(IDiscountRepository repository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName = {request.ProductName} not found."));

            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}",
                coupon.ProductName, coupon.Amount);
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _repository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _repository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
            ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            return new DeleteDiscountResponse
            {
                Success = deleted
            };
        }
    }
}