using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Umbraco.Core;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        private readonly NotificationService notificationService;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository, NotificationService notificationService)
        {
            this.orderRepository = orderRepository;
            this.bookRepository = bookRepository;
            this.notificationService = notificationService;
        }

        [HttpGet]
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

        private (Cart, Order) GetOrCreateCartAndOrder()
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

        private void SaveOrderAndCart(Cart cart, Order order)
        {
            orderRepository.Update(order);

            cart.TotalCount = order.TotalCount;
            cart.TotalPrice = order.TotalPrice;

            HttpContext.Session.Set(cart);
        }

        [HttpPost]
        public IActionResult AddItem(int id)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();

            var book = bookRepository.GetById(id);
            order.AddItem(book);

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Book", new { id });
        }

        [HttpPost]
        public IActionResult UpdateItem(int id, int count) //написать тест
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();
            if (order.TotalCount == 0)
            {
                var errorConfirmationModel = new ConfirmationModel();
                errorConfirmationModel.Errors["fatal"] = "Время сессии истекло...";
                return View("Confirmation", errorConfirmationModel);
            }

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

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();
            if (order.TotalCount == 0)
            {
                var errorConfirmationModel = new ConfirmationModel();
                errorConfirmationModel.Errors["fatal"] = "Время сессии истекло...";
                return View("Confirmation", errorConfirmationModel);
            }

            var book = bookRepository.GetById(id);
            order.DeleteItem(book);

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone, bool sendAgain = false)
        {
            var order = orderRepository.GetById(id);
            var errorConfirmationModel = new ConfirmationModel();

            if (order == null)
            {
                errorConfirmationModel.Errors["fatal"] = "Время сессии истекло...";
                return View("Confirmation", errorConfirmationModel);
            }

            var model = Map(order);
            if (cellPhone == null)
            {
                if (sendAgain)
                {
                    errorConfirmationModel.Errors["cellPhone"] = "Введите номер телефона";
                    return View("Confirmation", errorConfirmationModel);
                }
                model.Errors["cellPhone"] = "Введите номер телефона";
                return View("Index", model);
            }
            if (!IsValidCellPhone(cellPhone))
            {
                if (sendAgain)
                {
                    errorConfirmationModel.Errors["cellPhone"] = "Номер телефона не соответствует";
                    return View("Confirmation", errorConfirmationModel);
                }
                model.Errors["cellPhone"] = "Номер телефона не соответствует";
                return View("Index", model);
            }

            int code = 1111; //will be random(1000, 9999) soon
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(order, code);

            return View("Confirmation",
                        new ConfirmationModel
                        {
                            OrderId = order.Id,
                            CellPhone = cellPhone
                        });
        }

        public bool IsValidCellPhone(string cellPhone)
        {
            Regex regex = new Regex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"); //поправить
            if (regex.IsMatch(cellPhone))
                return true;
            return false;
        }

        [HttpPost]
        public IActionResult StartDelivery(int id, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone ?? "");
            var confirmationModel = new ConfirmationModel();
            if (storedCode == null)
            {
                confirmationModel.Errors["fatal"] = "Время сессии истекло...";
                return View("Confirmation", confirmationModel);
            }

            if (code.Equals(null))
            {
                return View("Confirmation",
                    new ConfirmationModel
                    {
                        OrderId = id,
                        CellPhone = cellPhone,
                        Errors = new Dictionary<string, string>
                                 { { "code", "Поле кода не заполненно" } }
                    });
            }

            if (code < 1000)
            {
                return View("Confirmation",
                    new ConfirmationModel
                    {
                        OrderId = id,
                        CellPhone = cellPhone,
                        Errors = new Dictionary<string, string>
                                 { { "code", "Поле кода заполненно неверно" } }
                    });
            }

            if (storedCode.Value != code)
            {
                return View("Confirmation",
                    new ConfirmationModel
                    {
                        OrderId = id,
                        CellPhone = cellPhone,
                        Errors = new Dictionary<string, string>
                                 { { "code", "Неверный код" } }
                    });
            }

            //
            return View();
        }
    }
}
