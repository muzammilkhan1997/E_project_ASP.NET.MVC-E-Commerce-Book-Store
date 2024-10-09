namespace E_Book_eproject.Models
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }


    }
}
