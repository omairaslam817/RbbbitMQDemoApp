namespace Receiver.Api.Models
{
    public class ProductResponseViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime PublsihedOn { get; set; }
        public List<ProductEventResponseModel> Events { get; set; } = new List<ProductEventResponseModel>();
    }
}
