using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQDemo.Core;
using Receiver.Api.Models.Entities;
using System.Security.Cryptography;

namespace Receiver.Api.Models.Features
{
    public class ProductViewedConsumer : IConsumer<ProductViewedEvent>
    {
        private readonly ApplicationDbContext _dbContext;

        //storing prouct in receiveer reporting api db
        public ProductViewedConsumer(ApplicationDbContext dbContext) //To store products in Product Repoerting service
        {
            _dbContext = dbContext;
        }
        public async Task Consume(ConsumeContext<ProductViewedEvent> context)
        {
            try
            {
                //This asssume that product previously created.it make sense beacise you can't view the article that wan't already creadted
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(product => product.Id == context.Message.Id); //match id to Message Id in context
                if (product is null)
                { //reason
                  //product view event out of order
                  //the product created event not processed
                    return;
                }
                var newProductEvent = new ProductEvent
                {
                     Name = context.Message.Name,
                    //Id = RandomNumberGenerator.GetInt32(0),
                    ProductId = product.Id, //enetity in this product report service db,altough same entity db exist in differnt db context in orignal microservice,
                                            //sharing data like this common in microservcies,and you dont have safty of Refrentail integrity,bcoz you are not working woth the same dbs,so get product entity id from current db,base on Message Id
                                            //this drawback of microservices
                    CreatedOn = context.Message.ViewedOn,
                    EventType = EventType.View
                };
                _dbContext.ProductEvents.Add(newProductEvent);
                await _dbContext.SaveChangesAsync(); //dont use canclation Token here bcaoz in reality I never want to cancel consuming the mesages,that come from message broker
                                                     //Add consumer to Masstrainet     x.AddConsumer<ProductViewedConsumer>(); in program.cs
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
