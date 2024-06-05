using Receiver.Api.Models.Entities;

namespace Receiver.Api.Models
{
    public class ProductEventResponseModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public EventType EventType { get; set; }
    }
}
