using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.Util;
using Microsoft.Data.SqlClient;
using System.Drawing.Printing;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/KindOfBook")]
    public class KindOfBookController : Controller
    {
        private readonly BookstoreContext _context;

        public KindOfBookController(BookstoreContext context)
        {
            _context = context;
        }

        // GET: KindOfBook
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var kindOfBooks = from k in _context.KindOfBooks
                           select k;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                kindOfBooks = kindOfBooks.Where(k => k.Name.Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    kindOfBooks = kindOfBooks.OrderByDescending(k => k.Name);
                    break;
                default:
                    kindOfBooks = kindOfBooks.OrderBy(k => k.Name);
                    break;
            }
            return View(await PaginatedList<KindOfBook>.CreateAsync(kindOfBooks.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        // GET: KindOfBook/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks
                .FirstOrDefaultAsync(m => m.KindOfBookId == id);
            if (kindOfBook == null)
            {
                return NotFound();
            }

            return View(kindOfBook);
        }

        // GET: KindOfBook/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: KindOfBook/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        public async Task<IActionResult> CreatePost([Bind("KindOfBookId,Name")] KindOfBook kindOfBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kindOfBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("create", kindOfBook);
        }

        // GET: KindOfBook/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks.FindAsync(id);
            if (kindOfBook == null)
            {
                return NotFound();
            }
            return View(kindOfBook);
        }

        // POST: KindOfBook/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        public async Task<IActionResult> EditPost([Bind("KindOfBookId,Name")] KindOfBook kindOfBook)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kindOfBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KindOfBookExists(kindOfBook.KindOfBookId))
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
            return View("edit", kindOfBook);
        }

        // GET: KindOfBook/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.KindOfBooks == null)
            {
                return NotFound();
            }

            var kindOfBook = await _context.KindOfBooks
                .FirstOrDefaultAsync(m => m.KindOfBookId == id);
            if (kindOfBook == null)
            {
                return NotFound();
            }

            return View(kindOfBook);
        }

        // POST: KindOfBook/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed([Bind("KindOfBookId")] KindOfBook kindOfBook)
        {
            if (_context.KindOfBooks == null)
            {
                return Problem("Entity set 'BookstoreContext.KindOfBooks'  is null.");
            }
            if (kindOfBook != null)
            {
                var sql = "";
                sql = "update bookshelf set kind_of_book_id=NULL where kind_of_book_id = {0}";
                await _context.Database.ExecuteSqlRawAsync(sql, kindOfBook.KindOfBookId);
                sql = "update book set kind_of_book_id=NULL where kind_of_book_id = {0}";
                await _context.Database.ExecuteSqlRawAsync(sql, kindOfBook.KindOfBookId);
                _context.KindOfBooks.Remove(kindOfBook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KindOfBookExists(int id)
        {
            return (_context.KindOfBooks?.Any(e => e.KindOfBookId == id)).GetValueOrDefault();
        }
    }
}
