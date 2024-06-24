namespace WebStore
{
    public interface IOrderRepository
    {
        Order Create();

        void Update(Order order);

        Order GetById(int id);
    }
}
