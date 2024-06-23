using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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
            if (HttpContext.Session.TryGetCart(out Cart cart))
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

        public IActionResult AddItem(int id)
        {
            var book = bookRepository.GetById(id);
            Order order;
            Cart cart;
            if (HttpContext.Session.TryGetCart(out cart))
            {
                order = orderRepository.GetById(cart.OrderId);
            }
            else
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }

            order.AddItem(book, 1);
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { id });
        }
    }
}
