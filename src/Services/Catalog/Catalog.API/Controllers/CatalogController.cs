using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly IProductRepository _repository;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return Ok(await _repository.GetProducts());
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id {id} do not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            return Ok(await _repository.GetProductByCategory(category));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> UpdateProduct(Product product)
        {
            return await _repository.UpdateProduct(product);
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            return await _repository.DeleteProduct(id);
        }
    }
}