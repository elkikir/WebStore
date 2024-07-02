using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Umbraco.Core;
using WebStore.Contractors;
using WebStore.WebMVC.Models;

namespace WebStore.WebMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IEnumerable<IDeliveryService> deliveryServices;
        private readonly NotificationService notificationService;

        public OrderController(IBookRepository bookRepository, IOrderRepository orderRepository,
            IEnumerable<IDeliveryService> deliveryServices, NotificationService notificationService)
        {
            this.orderRepository = orderRepository;
            this.bookRepository = bookRepository;
            this.deliveryServices = deliveryServices;
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
        public IActionResult UpdateItem(int id, int count)
        {
            (Cart cart, Order order) = GetOrCreateCartAndOrder();

            var book = bookRepository.GetById(id);

            try
            {
                if (count > 0)
                    order.AddItem(book, count);
                else if (order.Items.Count == 0) //попытка удаления товаров из пустой корзины
                {
                    return View("Error", new ErrorViewModel
                    {
                        Errors = new Dictionary<string, string>
                                 { { "fatal", "Время сессии истекло" } }
                    });
                }
                else if (count < 0)
                    order.RemoveItem(book, count);
                else
                    order.DeleteItem(book);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Order");
            }

            SaveOrderAndCart(cart, order);

            return RedirectToAction("Index", "Order");
        }

        public IActionResult Confirmation(int id)
        {
            if (orderRepository.GetById(id) == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new Dictionary<string, string>
                                 { { "fatal", "Время сессии истекло" } }
                });
            }

            return View("Confirmation",
                        new ConfirmationModel
                        {
                            OrderId = id,
                            CellPhone = null
                        });
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(int id, string cellPhone)
        {
            var order = orderRepository.GetById(id);
            var model = new ConfirmationModel
            {
                OrderId = id,
                CellPhone = cellPhone
            };

            if (order == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new Dictionary<string, string>
                                 { { "fatal", "Время сессии истекло" } }
                });
            }

            if (cellPhone == null)
            {
                model.Errors["phone"] = "Введите номер телефона";
                return View("Confirmation", model);
            }
            if (!IsValidCellPhone(cellPhone))
            {
                model.Errors["phone"] = "Номер телефона не соответствует";
                return View("Confirmation", model);
            }

            int code = 1111; //will be random(1000, 9999) soon
            HttpContext.Session.SetInt32(cellPhone, code);
            notificationService.SendConfirmationCode(order, code);

            return View("Confirmation", model);
        }

        public bool IsValidCellPhone(string cellPhone)
        {
            Regex regex =
                new Regex(@"^((8|\+374|\+994|\+995|\+375|\+7|\+380|\+38|\+996|\+998|\+993)[\- ]?)?\(?\d{3,5}\)?[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}[\- ]?\d{1}(([\- ]?\d{1})?[\- ]?\d{1})?$");
            if (regex.IsMatch(cellPhone))
                return true;
            return false;
        }

        [HttpPost]
        public IActionResult Confirmate(int id, string cellPhone, int code)
        {
            int? storedCode = HttpContext.Session.GetInt32(cellPhone ?? "");

            if (storedCode == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Errors = new Dictionary<string, string>
                                 { { "fatal", "Время сессии истекло" } }
                });
            }

            if (code == 0)
            {
                return View("Confirmation",
                    new ConfirmationModel
                    {
                        OrderId = id,
                        CellPhone = cellPhone,
                        Errors = new Dictionary<string, string>
                                 { { "code", "Введите код" } }
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
                                 { { "code", "Поле кода заполнено некорректно" } }
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

            //todo: save telephone 

            HttpContext.Session.Remove(cellPhone);

            var model = new DeliveryModel
            {
                OrderId = id,
                Methods = deliveryServices.ToDictionary(service => service.UniqueCode,
                                                        service => service.Title)
            };

            return View("DeliveryMethod", model);
        }

        [HttpPost]
        public IActionResult StartDelivery(int id, string uniqueCode)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);
            var order = orderRepository.GetById(id);

            var form = deliveryService.CreateForm(order);

            return View("DeliveryStep", form);
        }

        [HttpPost]
        public IActionResult NextDelivery(int id, string uniqueCode, int step, Dictionary<string, string> values)
        {
            var deliveryService = deliveryServices.Single(service => service.UniqueCode == uniqueCode);

            var form = deliveryService.MoveNext(id, step, values);

            if(form.IsFinal)
            {
                return null;
            }

            return View("DeliveryStep", form);
        }
    }
}
