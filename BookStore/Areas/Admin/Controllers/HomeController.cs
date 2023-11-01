using BookStore.CustomAtrribute;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class HomeController : Controller
    {
        [Role("ROLE_ADMIN")]
        [Role("ROLE_MANAGER")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
