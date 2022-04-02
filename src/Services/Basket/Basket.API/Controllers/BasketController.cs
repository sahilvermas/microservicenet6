using System;
using System.Net;
using System.Threading.Tasks;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IBasketRepository _repository;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
        {
            return Ok(await _repository.GetBasket(username) ?? new ShoppingCart(username));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string username)
        {
            await _repository.DeleteBasket(username);
            return Ok();
        }
    }
}