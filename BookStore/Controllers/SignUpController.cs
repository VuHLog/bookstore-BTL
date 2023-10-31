using BookStore.Data;
using BookStore.DTO;
using BookStore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class SignUpController : Controller
    {
        private readonly BookstoreContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public SignUpController(BookstoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Route("signup")]
        public IActionResult Index()
        {
            ProfileDTO profile = new ProfileDTO();
            return View(profile);
        }

        [Route("createAcount")]
        public async Task<IActionResult> createAccount([Bind("UserId, CustomerId,avatar,Firstname,Lastname,Username,Password,Email,gender,dateOfBirth,address")] ProfileDTO profile,string EnterPasswordAgain)
        {
            if (ModelState.IsValid)
            {

                User username = await (from u in _context.Users
                                         where u.Username == profile.Username
                                         select u).FirstOrDefaultAsync();

                if(username != null)
                {
                    ViewBag.UserExist = "Tên tài khoản đã tồn tại!";
                    return View("Index");
                }

                User user = profile.getUser();
                if(user.Password != EnterPasswordAgain)
                {
                    ViewBag.PassNoMatch = "Tên tài khoản đã tồn tại!";
                    return View("Index");
                }
                //encode password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                if (profile.avatar != null)
                {

                    string folder = "images\\user\\" + Guid.NewGuid().ToString() + profile.avatar.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    user.avatarUrl = "\\" + folder;

                    await profile.avatar.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }
                else
                {
                    user.avatarUrl = "\\images\\user\\user-default.png";
                }

                _context.Add(user);
                await _context.SaveChangesAsync();


                var sql = "INSERT INTO users_roles (user_id, role_id) VALUES ({0},{1})";
                await _context.Database.ExecuteSqlRawAsync(sql, user.UserId, 5);

                Customer customer = profile.getCustomer();
                customer.UserId = user.UserId;
                _context.Add(customer);
                await _context.SaveChangesAsync();

                return Redirect("/login");
            }
            return View("Index", profile);
        }
    }
}
