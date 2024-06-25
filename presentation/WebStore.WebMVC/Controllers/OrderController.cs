using Microsoft.AspNetCore.Mvc;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
            this.bookRepository = bookRepository;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.TryGetCart(out Cart cart) && cart.TotalCount > 0)
            {
                var order = orderRepository.GetById(cart.OrderId);
                OrderModel model = Map(order);

                return View(model);
            }
            return View("Empty");
        }

        private OrderModel Map(Order order)
        {
            var bookId = order.Items.Select(item => item.BookId);
            var books = bookRepository.GetAllById(bookId);
            var itemModels = from item in order.Items
                             join book in books on item.BookId equals book.Id
                             select new OrderItemModel
                             {
                                 BookId = book.Id,
                                 Title = book.Title,
                                 Author = book.Author,
                                 Price = item.Price,
                                 Count = item.Count,
                             };

            return new OrderModel
            {
                Id = order.Id,
                Item = itemModels.ToArray(),
                TotalCount = order.TotalCount,
                TotalPrice = order.TotalPrice,
            };
        }

        public (Cart, Order) GetOrCreateCartAndOrder()
        {
            Order order;

            if (HttpContext.Session.TryGetCart(out Cart cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }

            return (cart, order);
        }

        public void SaveOrderAndCart(Cart cart, Order order)
        {
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }

        public IActionResult AddItem(int id)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();

            var book = bookRepository.GetById(id);
            order.AddItem(book);

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Book", new { id });
        }

        public IActionResult UpdateItem(int id, int count)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();

            var book = bookRepository.GetById(id);
            switch (order.GetItem(id).Count + count)
            {
                case int final when final > 0:
                    order.AddItem(book, count);
                    break;
                case int final when final == 0:
                    order.DeleteItem(book); 
                    break;
                case int final when final < 0:
                    throw new InvalidOperationException("Попытка удаления объекта не находящегося в корзине");
            }

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Order");
        }

        public IActionResult DeleteItem(int id)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();

            var book = bookRepository.GetById(id);
            order.DeleteItem(book);

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Order");
        }
    }
}
