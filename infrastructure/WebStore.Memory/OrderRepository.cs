namespace WebStore.Memory
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List<Order> orders = new List<Order>();

        public Order Create()
        {
            int nextId = orders.Count + 1;
            var order = new Order(nextId, new OrderItem[0]);

            orders.Add(order);
            return order;
        }

        public Order GetById(int id)
        {
            if (orders.Count > 0 && id > 0)
                return orders.Single(order => order.Id == id);
            else
                return null;
        }

        public void Update(Order order)
        {
            ;
        }
    }
}
