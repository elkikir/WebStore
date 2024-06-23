namespace WebStore.WebMVC.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public OrderItemModel[] Item { get; set; } = new OrderItemModel[0];
        public int TotalCount {  get; set; }
        public decimal TotalPrice { get; set; }
    }
}
