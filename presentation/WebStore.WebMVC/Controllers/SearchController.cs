using Microsoft.AspNetCore.Mvc;

namespace WebStore.WebMVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly IBookRepository bookRepository;

        public SearchController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        public IActionResult Index(string query)
        {
            var book = bookRepository.GetAllBooksByTitle(query);

            return View(book);
        }
    }
}
