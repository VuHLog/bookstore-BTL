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
        public async Task<IActionResult> Index()
        {
            //dem so user
            int userCount = (from c in _context.Customers select c).Count();
            ViewBag.UserCount = userCount;

            //dem so sach
            int bookCount = (from b in _context.Books select b).Count();
            ViewBag.BookCount = bookCount;

            //dem so don hang
            int invoiceOutCount = (from i in _context.InvoicesOuts select i).Count();
            ViewBag.InvoiceOutCount = invoiceOutCount;
            return View();
        }
    }
}
