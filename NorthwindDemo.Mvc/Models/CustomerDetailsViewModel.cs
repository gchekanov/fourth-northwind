namespace NorthwindDemo.Mvc.Models
{
    public class CustomerDetailsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<OrderViewModel> Orders { get; set; } = new();

    }
}
