namespace RabbitMQDemo.Core
{
    public record ProductCreatedEvent //Use sperate class libry proj for Message contrct ,becuase i dont want that my Apis know about each other
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public record ProductViewedEvent //Use sperate class libry proj for Message contrct ,becuase i dont want that my Apis know about each other
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public DateTime ViewedOn { get; set; }

    }
}
