using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.Data;
using Microsoft.AspNetCore.Hosting;
using BookStore.CustomAtrribute;

namespace BookStore.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Book")]
    public class BookController : Controller
    {
        private readonly BookstoreContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(BookstoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Books
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index()
        {
            var bookstoreContext = _context.Books.Include(b => b.KindOfBook);
            return View(await bookstoreContext.ToListAsync());
        }

        // GET: Books/Details/5
        [Route("Details/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.KindOfBook)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["KindOfBookId"] = new SelectList(_context.KindOfBooks, "KindOfBookId", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("BookId,Authors,Cost,Date,Name,Number,KindOfBookId,Content,productImage")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.productImage != null)
                {

                    string folder = "images\\books\\" + Guid.NewGuid().ToString() + book.productImage.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

                    book.imageUrl = "\\" + folder;

                    await book.productImage.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KindOfBookId"] = new SelectList(_context.KindOfBooks, "KindOfBookId", "Name", book.KindOfBookId);
            return View("create", book);
        }

        // GET: Books/Edit/5
        [Route("Edit/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["KindOfBookId"] = new SelectList(_context.KindOfBooks, "KindOfBookId", "Name", book.KindOfBookId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("BookId,Authors,Cost,Date,Name,Number,KindOfBookId,Content,productImage,imageUrl")] Book book)
        {
            if (ModelState.IsValid)
            {

                if (book.productImage != null)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    //string oldFilePath = webRootPath + user.avatarUrl;
                    //if (System.IO.File.Exists(oldFilePath))
                    //{
                    //    System.IO.File.Delete(oldFilePath); // Xóa tệp
                    //}

                    string folder = "images\\user\\" + Guid.NewGuid().ToString() + book.productImage.FileName;
                    string serverFolder = Path.Combine(webRootPath, folder);

                    book.imageUrl = "\\" + folder;

                    await book.productImage.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                }

                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["KindOfBookId"] = new SelectList(_context.KindOfBooks, "KindOfBookId", "Name", book.KindOfBookId);
            return View("edit", book);
        }

        // GET: Books/Delete/5
        [Route("Delete/{id}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.KindOfBook)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("BookId")] Book book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookstoreContext.Books'  is null.");
            }
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
