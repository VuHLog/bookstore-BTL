﻿using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

namespace BookStore.Controllers
{
    public class SigninController : Controller
    {
        private readonly BookstoreContext _context;

        public SigninController(BookstoreContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        //POST : login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([Bind("Password,Username")] User userRq)
        {   
            //Validate
            if(!ModelState.IsValid)
            {
                return View(new User());
            }
            else
            {
                //find username
                User? user = (from u in _context.Users
                             where u.Username == userRq.Username
                             select u).FirstOrDefault();
                if (user == null)
                {
                    ViewBag.error = "Tên tài khoản hoặc mật khẩu không đúng!";
                    return View("Login");
                }
                // so sanh mat khau nguoi dung nhap voi mat khau duoc ma hoa bcrypt trong db
                if(!BCrypt.Net.BCrypt.Verify(userRq.Password, user.Password))
                {
                    ViewBag.error = "Tên tài khoản hoặc mật khẩu không đúng!";
                    return View("Login");
                }
                var account = (from userRole in _context.UsersRoles
                              join u in _context.Users on userRole.UserId equals u.Id
                              join role in _context.Roles on userRole.RoleId equals role.Id
                              where u.Id == user.Id
                              select new
                              {
                                  username = user.Username,
                                  fullname = user.Lastname +" "+ user.Firstname,
                                  role = role.Name
                              }).FirstOrDefault();
                Response.Cookies.Append("account", JsonConvert.SerializeObject(account));
                //return ve trang truoc do
                string returnUrl = Request.Headers["Referer"].ToString();
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }


    }
}
