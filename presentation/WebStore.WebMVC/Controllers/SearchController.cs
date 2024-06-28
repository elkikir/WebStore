using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookService bookService;

        public SearchController(BookService bookService)
        {
            this.bookService = bookService;
        }

        public IActionResult Index(string query)
        {
            if (query == null)
                return View(new Book[0]);

            var book = bookService.GetAllByQuery(query);

            return View(book);
        }
    }
}
