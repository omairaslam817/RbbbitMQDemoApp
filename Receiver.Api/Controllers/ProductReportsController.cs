using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Receiver.Api.Models;
using Receiver.Api.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Receiver.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductReportsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: api/<ProductReportsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductReportsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var arcticleResponse = await _dbContext.Products.AsNoTracking()
                .Where(product => product.Id == id)
                .Select(prodcut=> new ProductResponseViewModel
                {
                     Id = prodcut.Id,
                     CreatedOn = prodcut.CreatedOn,
                     PublsihedOn = prodcut.PublsihedOn,
                     Events = _dbContext.ProductEvents
                     .AsNoTracking()
                     .Where (productEvent => productEvent.ProductId == id)
                     .Select(productEvent=> new ProductEventResponseModel
                     {
                          Id = productEvent.Id,
                          EventType = productEvent.EventType,
                          CreatedOn = productEvent.CreatedOn,

                     }).ToList()

                }).FirstOrDefaultAsync();
            if (arcticleResponse == null)
            {
                return NotFound("the product with specified id not found");
            }
            return Ok(arcticleResponse);
        }

        // POST api/<ProductReportsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProductReportsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductReportsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
