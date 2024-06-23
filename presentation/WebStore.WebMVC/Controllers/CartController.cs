using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStore.Memory;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;

        public CartController(IBookRepository bookRepository, IOrderRepository orderRepository)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
        }

        public IActionResult Add(int id)
        {
            Order order;
            Cart cart;
            if (!HttpContext.Session.TryGetCart(out cart))
            {
                order = orderRepository.Create();
                cart = new Cart(order.Id);
            }
            else
            {
                order = orderRepository.GetById(cart.OrderId);
            }

            var book = bookRepository.GetById(id);
            order.AddItem(book, 1);
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);

            return RedirectToAction("Index", "Book", new { id });
        }
    }
}
