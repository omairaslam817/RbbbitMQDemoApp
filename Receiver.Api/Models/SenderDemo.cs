using MassTransit;
using RabbitMQDemo.Core;

namespace Receiver.Api.Models
{
    public class SenderDemo : IConsumer<ProductCreatedEvent>
    {
        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var product = context.Message;
        }
    }
}
