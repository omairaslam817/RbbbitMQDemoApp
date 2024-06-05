using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQDemo.Core;
using Receiver.Api.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sender.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQDemoController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ApplicationDbContext _dbContext;

        public RabbitMQDemoController(IPublishEndpoint  publishEndpoint,
            ApplicationDbContext dbContext) //Inject MassTransinent Bus
        {
            _publishEndpoint = publishEndpoint;
            _dbContext = dbContext;
        }
        // GET: api/<RabbitMQDemoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RabbitMQDemoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _dbContext.Products.Where(m => m.Id == id).Select(//1- query for product
                prod => new 
                {
                    Id = prod.Id,
                    Name = prod.Name,
                    Price = prod.Price,
                    ViewOn = prod.CreatedOn
                }).FirstOrDefaultAsync();

            await _publishEndpoint.Publish(new ProductViewedEvent  //2 Publish the event that has been viewed when we hit get by id api
            {
                Id = product.Id,
                Name = product.Name,

                Price = product.Price,
                ViewedOn = product.ViewOn
            });
            //Services only know each other base on Message contracts ProductViewedEvent
            return Ok(product);//return prodcut to api consumer
        }

        // POST api/<RabbitMQDemoController>
        [HttpPost("send-demo")]
        public async Task<IActionResult> Post([FromBody] ProductCreatedEvent model)
        {
            //Command Send Part
            Product product = new Product()
            {
                //1- Sender App is Producer which Publish message to an Exchange.
                //2- Producer Request come to Exchange,when creating exchnage type must be specify,
                //3- The exchange receive the message and is now responsible for routing the message. exchange include differnt parts such as routing key,depend on exchange type
                //3- Then Binding must be created fromExchange to Queues.In our case we have Two bindings,Two differnt Queues from exchange.
                //Then Exchange routes the message into the queues depen on message attributes.
                //4- Then Consumer/Subscriber conumse the application. 
                //5- Then Consumer receive the application
                Name = model.Name,
                Price = model.Price,
            };
            var url = new Uri("rabbitmq://localhost/send-demo"); //send-demo is Exchange

          
            //Persist in Db Product
            var productEntityResult = _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            await _publishEndpoint.Publish(new ProductCreatedEvent //consumer will be listening ProductCreatedEvent //Publishing respective event over the message broker
            { //Publishing the events,Becuase we are using Events ,Ourcomminication is loosely couple,Services dont know about eachother,They only know about message contrcts,which are actual events,
                Id = productEntityResult.Entity.Id,
                Name = model.Name,
                Price = model.Price,
            });
            return Ok(product); 
        }

        // PUT api/<RabbitMQDemoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RabbitMQDemoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
