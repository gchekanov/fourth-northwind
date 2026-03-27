namespace NorthwindDemo.Mvc.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool HasIssues { get; set; }
        public List<string> Issues { get; set; } = new();

    }
}
