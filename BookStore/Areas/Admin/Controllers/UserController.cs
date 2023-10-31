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
using BookStore.CustomAtrribute;

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
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
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
            ViewBag.RoleSortParam = sortOrder == "Role" ? "role_desc" : "Role";

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
                                       || u.Enabled.ToString().Contains(searchString.ToString())
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
        [Route("Details/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Role = await _context.Roles.ToListAsync();
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("UserId,Email,Enabled,Firstname,Lastname,Password,Username,avatar")] User user, int role)
        {
            // luu anh vao thu muc images/user
            if (ModelState.IsValid)
            {
                
                string username =await (from u in _context.Users
                                  where u.Username == user.Username
                                  select u.Username).FirstOrDefaultAsync();
                if(username == null)
                {
                    if (user.avatar != null)
                    {

                        string folder = "images\\user\\" + Guid.NewGuid().ToString() + user.avatar.FileName;
                        string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                        user.avatarUrl = "\\" + folder;

                        await user.avatar.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    }
                    else
                    {
                        user.avatarUrl = "\\images\\user\\user-default.png";
                    }
                    //encode password
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    _context.Add(user);
                    await _context.SaveChangesAsync();


                    using (_context)
                    {
                        var sql = "INSERT INTO users_roles (user_id, role_id) VALUES ({0},{1})";
                        await _context.Database.ExecuteSqlRawAsync(sql, user.UserId, role);
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return View("create", user);
        }

        // GET: User/Edit/5
        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
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
            ViewBag.RoleCurr = await (from ur in _context.UsersRoles
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where ur.UserId == id
                                      select r
                                      ).FirstOrDefaultAsync() ;
            ViewBag.Role = await _context.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("UserId,Email,Enabled,Firstname,Lastname,Password,Username,avatar,avatarUrl")] User user,int? role)
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
                    if (role != null)
                    {
                        using (_context)
                        {
                            var sql = "update users_roles set role_id = {0} where user_id={1}";
                            await _context.Database.ExecuteSqlRawAsync(sql, role, user.UserId);
                        }
                    }
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
                return RedirectToAction(nameof(Index));
            }
            return View("edit", user);
        }

        // GET: User/Delete/5
        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
            ViewBag.RoleCurr = await (from ur in _context.UsersRoles
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where ur.UserId == id
                                      select r
                                      ).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("UserId")] User user,int? role)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'BookstoreContext.Users'  is null.");
            }
            if (user != null)
            {
                    var sql = "Delete from users_roles where user_id = {0} and role_id = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, user.UserId, role);
                    _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
