namespace Receiver.Api.Models.Entities
{
    public class ProductEvent
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public EventType EventType { get; set; }
    }
    public enum EventType
    {
        View =1
    }
}
