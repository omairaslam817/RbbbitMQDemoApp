using MassTransit;
using RabbitMQDemo.Core;
using Receiver.Api.Models.Entities;

namespace Receiver.Api.Models.Features
{
    public sealed class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>//When Product created event Published,its going to first hit the Message Queue which is RabbitMQ,and MassTransinet is going to take care of subscribing to this Messag in Product Reporting Api,it going to invoke ProductCreatedConsumer
    {
        private readonly ApplicationDbContext _dbContext;

        //storing prouct in receiveer reporting api db
        public ProductCreatedConsumer(ApplicationDbContext dbContext) //To store products in Product Repoerting service
        {
            _dbContext = dbContext;
        }
        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var product = new Product()
            {
                //Id = context.Message.Id, //Message is Masstrainet rabbitmq object
                Name = context.Message.Name,
                Price = context.Message.Price,
            };
            _dbContext.Add(product);
            await _dbContext.SaveChangesAsync();
            await context.Send(product);
        }
        //1- when published ProductCreatedEvent first it hits the Message Queue,which is RabbitMQ
        //2- Then MassTrainet it is going to take care of subscribing to this Message in
        //Product receiver Reporting Api and It will invoke the ProductCreatedConsumer
    }
}
