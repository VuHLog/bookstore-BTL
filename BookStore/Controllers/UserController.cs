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

namespace BookStore.Controllers
{
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
        [Route("admin/user")]
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'BookstoreContext.Users'  is null.");
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("Id,Email,Enabled,Firstname,Lastname,Password,Username,avatar")] User user)
        {
            // luu anh vao thu muc images/user
            if (ModelState.IsValid)
            {
                if (user.avatar != null)
                {

                    string folder = "images\\user\\"+ Guid.NewGuid().ToString() + user.avatar.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    user.avatarUrl = "\\"+ folder;

                    await user.avatar.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }
                //encode password
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("create",user);
        }

        // GET: User/Edit/5
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

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            return View("edit",user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'BookstoreContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
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
