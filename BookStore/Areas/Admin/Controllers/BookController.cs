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
using BookStore.Util;
using System.Globalization;

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
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParam = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewBag.NameSortParam = sortOrder == "Name" ? "name_desc" : "";
            ViewBag.AuthorsSortParam = sortOrder == "Authors" ? "authors_desc" : "Authors";
            ViewBag.CostSortParam = sortOrder == "Cost" ? "cost_desc" : "Cost";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.NumberSortParam = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.ContentSortParam = sortOrder == "Content" ? "content_desc" : "Content";
            ViewBag.KindOfBookNameSortParam = sortOrder == "KindOfBookName" ? "kindOfBookName_desc" : "KindOfBookName";

            //luu filter hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var books = from b in _context.Books.Include(b => b.KindOfBook)
                           select b;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize;

            //search
            if (!String.IsNullOrEmpty(searchString))
            {
                
                books = books.Where(b =>b.BookId.ToString().Contains(searchString.ToString())
                                       || b.Name.Contains(searchString)
                                       || b.KindOfBook.Name.Contains(searchString)
                                       || b.Cost.ToString().Contains(searchString.ToString())
                                       || b.Date.ToString().Contains(searchString.ToString())
                                       || b.Authors.Contains(searchString)
                                       || b.Content.Contains(searchString)
                                       || b.Number.ToString().Contains(searchString.ToString())
                                       );
            }
            //xác định tên trường sort
            switch (sortOrder)
            {
                case "id_desc":
                    books = books.OrderByDescending(b => b.BookId);
                    break;
                case "name_desc":
                    books = books.OrderByDescending(b => b.Name);
                    break;
                case "Name":
                    books = books.OrderBy(b => b.Name);
                    break;
                case "Number":
                    books = books.OrderBy(b => b.Number);
                    break;
                case "number_desc":
                    books = books.OrderByDescending(b => b.Number);
                    break;
                case "Cost":
                    books = books.OrderBy(b => b.Cost);
                    break;
                case "cost_desc":
                    books = books.OrderByDescending(b => b.Cost);
                    break;
                case "Date":
                    books = books.OrderBy(b => b.Date);
                    break;
                case "date_desc":
                    books = books.OrderByDescending(b => b.Date);
                    break;
                case "Authors":
                    books = books.OrderBy(b => b.Authors);
                    break;
                case "authors_desc":
                    books = books.OrderByDescending(b => b.Cost);
                    break;
                case "Content":
                    books = books.OrderBy(b => b.Content);
                    break;
                case "Content_desc":
                    books = books.OrderByDescending(b => b.Cost);
                    break;
                case "KindOfBookName":
                    books = books.OrderBy(b => b.KindOfBook.Name);
                    break;
                case "kindOfBookName_desc":
                    books = books.OrderByDescending(b => b.KindOfBook.Name);
                    break;
                default:
                    books = books.OrderBy(b => b.BookId);
                    break;
            }
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
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
                .FirstOrDefaultAsync(b => b.BookId == id);
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
                    //if (Systeb.IO.File.Exists(oldFilePath))
                    //{
                    //    Systeb.IO.File.Delete(oldFilePath); // Xóa tệp
                    //}

                    string folder = "images\\books\\" + Guid.NewGuid().ToString() + book.productImage.FileName;
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
                .FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        [Route("DeleteConfirmed")]
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
