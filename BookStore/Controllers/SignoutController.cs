using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class SignoutController : Controller
    {
        [Route("/signout")]
        [Route("admin/signout")]
        public IActionResult Index()
        {
            string returnUrl = Request.Headers["Referer"].ToString();
            Response.Cookies.Delete("account");
            if (returnUrl != string.Empty)
                return Redirect(returnUrl);
            else return RedirectToAction("Index", "Home");
        }
    }
}
