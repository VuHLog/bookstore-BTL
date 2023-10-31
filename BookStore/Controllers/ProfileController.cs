using BookStore.CustomAtrribute;
using BookStore.Data;
using BookStore.DTO;
using BookStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BookstoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProfileController(BookstoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Role("ROLE_USER")]
        [Role("ROLE_ADMIN")]
        [Route("/profile/{username}")]
        public async Task<IActionResult> Index(string username)
        {
            string passError = TempData["PassError"] as string;
            if (!string.IsNullOrEmpty(passError))
            {
                ViewBag.PassError = passError;
            }

            string passNoMatch = TempData["PassNoMatch"] as string;
            if (!string.IsNullOrEmpty(passNoMatch))
            {
                ViewBag.PassNoMatch = passNoMatch;
            }

            string changePasswordSuccess = TempData["ChangePasswordSuccess"] as string;
            if (!string.IsNullOrEmpty(changePasswordSuccess))
            {
                ViewBag.ChangePasswordSuccess = changePasswordSuccess;
            }

            User user =await (from u in _context.Users
                        where u.Username == username
                        select u).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            Customer customer = await (from u in _context.Users
                                join c in _context.Customers on u.UserId equals c.UserId
                                where c.UserId == user.UserId
                                select c).FirstOrDefaultAsync();

            ProfileDTO profile = new ProfileDTO();
            profile.setUser(user);
            if(customer!= null)
            {
                profile.setCustomer(customer);
            }
            return View(profile);
        }

        [Role("ROLE_USER")]
        [Role("ROLE_ADMIN")]
        [HttpPost]
        [Route("saveInfo")]
        public async Task<IActionResult> saveInfo([Bind("UserId, CustomerId,Email,Firstname,Lastname,Password,Username,avatar,address,dateOfBirth,gender,avatarUrl")] ProfileDTO profile)
        {
            if (ModelState.IsValid)
            {
                User user = profile.getUser();
                if (profile.avatar != null)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;

                    string folder = "images\\user\\" + Guid.NewGuid().ToString() + user.avatar.FileName;
                    string serverFolder = Path.Combine(webRootPath, folder);

                    user.avatarUrl = "\\" + folder;

                    await user.avatar.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Customer customer = profile.getCustomer();
                if (customer != null)
                {
                    try
                    {
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CustomerExists(customer.CustomerId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return Redirect("/home");
            }
            return View("index", profile);
        }

        [Role("ROLE_USER")]
        [Role("ROLE_ADMIN")]
        [HttpPost]
        [Route("changePassword")]
        public async Task<IActionResult> changePassword(long userId, string password,string newPassword,string EnterPasswordAgain)
        {
            User user = await (from u in _context.Users
                        where u.UserId == userId
                        select u).FirstOrDefaultAsync();

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                TempData["PassError"] = "Mật khẩu không đúng!";
                return Redirect("/profile/" + user.Username);
            }

            if(newPassword != EnterPasswordAgain)
            {
                TempData["PassNoMatch"] = "Mật khẩu không khớp!";
                return Redirect("/profile/" + user.Username);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Update(user);

            TempData["ChangePasswordSuccess"] = "Thay đổi mật khẩu thành công!";
            return Redirect("/profile/" + user.Username);
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }

    }
}
