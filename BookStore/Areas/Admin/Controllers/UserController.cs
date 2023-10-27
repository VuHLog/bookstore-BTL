using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using System.IO;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Drawing;
using BookStore.Data;
using BookStore.Util;

namespace BookStore.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly BookstoreContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(BookstoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: User
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewBag.LastNameSortParam = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewBag.EmailSortParam = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.UsernameSortParam = sortOrder == "Username" ? "username_desc" : "Username";
            ViewBag.PasswordSortParam = sortOrder == "Password" ? "password_desc" : "Password";
            ViewBag.EnabledSortParam = sortOrder == "Enabled" ? "enabled_desc" : "Enabled";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var users = from u in _context.Users
                           select u;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Firstname.Contains(searchString)
                                       || u.Lastname.Contains(searchString)
                                       || u.Email.Contains(searchString)
                                       || u.Username.Contains(searchString)
                                       || u.Enabled.ToString().Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "firstname_desc":
                    users = users.OrderByDescending(u => u.Firstname);
                    break;
                case "LastName":
                    users = users.OrderBy(u => u.Lastname);
                    break;
                case "LastName_desc":
                    users = users.OrderByDescending(u => u.Lastname);
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case "Username":
                    users = users.OrderBy(u => u.Username);
                    break;
                case "username_desc":
                    users = users.OrderByDescending(u => u.Username);
                    break;
                case "Password":
                    users = users.OrderBy(u => u.Password);
                    break;
                case "password_desc":
                    users = users.OrderByDescending(u => u.Password);
                    break;
                case "Enabled":
                    users = users.OrderBy(u => u.Password);
                    break;
                case "enabled_desc":
                    users = users.OrderByDescending(u => u.Enabled);
                    break;
                default:
                    users = users.OrderBy(u => u.Firstname);
                    break;
            }
            return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        // GET: User/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        public async Task<IActionResult> CreatePost([Bind("Id,Email,Enabled,Firstname,Lastname,Password,Username,avatar")] User user)
        {
            // luu anh vao thu muc images/user
            if (ModelState.IsValid)
            {
                if (user.avatar != null)
                {

                    string folder = "images\\user\\" + Guid.NewGuid().ToString() + user.avatar.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    user.avatarUrl = "\\" + folder;

                    await user.avatar.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }
                //encode password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("create", user);
        }

        // GET: User/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        public async Task<IActionResult> EditPost([Bind("Id,Email,Enabled,Firstname,Lastname,Password,Username,avatar,avatarUrl")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.avatar != null)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    //string oldFilePath = webRootPath + user.avatarUrl;
                    //if (System.IO.File.Exists(oldFilePath))
                    //{
                    //    System.IO.File.Delete(oldFilePath); // Xóa tệp
                    //}

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
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("edit", user);
        }

        // GET: User/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("UserId")] User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'BookstoreContext.Users'  is null.");
            }
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
