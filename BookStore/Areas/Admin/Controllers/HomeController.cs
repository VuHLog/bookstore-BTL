using BookStore.CustomAtrribute;
using BookStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class HomeController : Controller
    {
        private readonly BookstoreContext _context;

        public HomeController(BookstoreContext context)
        {
            _context = context;
        }
        [Role("ROLE_ADMIN")]
        [Role("ROLE_MANAGER")]
        [Route("Home")]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
