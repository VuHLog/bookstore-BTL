using BookStore.CustomAtrribute;
using BookStore.Data;
using BookStore.Models;
using BookStore.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/BookInvoicesOut")]
    public class BookInvoicesOutController : Controller
    {
        private readonly BookstoreContext _context;

        public BookInvoicesOutController(BookstoreContext context)
        {
            _context = context;
        }

        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoicesOutIdSortParam = String.IsNullOrEmpty(sortOrder) ? "invoicesoutid_desc" : "";
            ViewBag.BookIdSortParam = sortOrder == "BookId" ? "bookid_desc" : "BookId";
            ViewBag.BookNameSortParam = sortOrder == "BookName" ? "bookname_desc" : "BookName";
            ViewBag.QuantitySortParam = sortOrder == "Quantity" ? "quantity_desc" : "quantity";

            //luu bo loc hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var bookInvoicesOuts = from i in _context.BookInvoicesOuts.Include(i => i.InvoicesOut).Include(i => i.Book)
                               select i;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                bookInvoicesOuts = bookInvoicesOuts.Where(i => i.InvoicesOutId.ToString().Contains(searchString.ToString())
                                                || i.BookId.ToString().Contains(searchString.ToString())
                                                || i.Book.Name.Contains(searchString)
                                                || i.Quantity.ToString().Contains(searchString.ToString())
                                       );
            }

            switch (sortOrder)
            {
                case "invoicesoutid_desc":
                    bookInvoicesOuts = bookInvoicesOuts.OrderByDescending(m => m.InvoicesOutId);
                    break;
                case "BookId":
                    bookInvoicesOuts = bookInvoicesOuts.OrderBy(m => m.BookId);
                    break;
                case "bookid_desc":
                    bookInvoicesOuts = bookInvoicesOuts.OrderByDescending(m => m.BookId);
                    break;
                case "BookName":
                    bookInvoicesOuts = bookInvoicesOuts.OrderBy(m => m.Book.Name);
                    break;
                case "bookname_desc":
                    bookInvoicesOuts = bookInvoicesOuts.OrderByDescending(m => m.Book.Name);
                    break;
                case "Quantity":
                    bookInvoicesOuts = bookInvoicesOuts.OrderBy(m => m.Quantity);
                    break;
                case "quantity_desc":
                    bookInvoicesOuts = bookInvoicesOuts.OrderByDescending(m => m.Quantity);
                    break;
                default:
                    bookInvoicesOuts = bookInvoicesOuts.OrderBy(m => m.InvoicesOutId);
                    break;
            }
            return View(await PaginatedList<BookInvoicesOut>.CreateAsync(bookInvoicesOuts.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }


        [Route("Details/{invoicesOutId}/{bookId}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? invoicesOutId, int? bookId)
        {
            if (invoicesOutId == null || bookId == null || _context.BookInvoicesOuts == null)
            {
                return NotFound();
            }
            var BookInvoicesOut = await (from bi in _context.BookInvoicesOuts
                                        where bi.BookId == bookId && bi.InvoicesOutId == invoicesOutId
                                        select bi).FirstOrDefaultAsync();
            if (BookInvoicesOut == null)
            {
                return NotFound();
            }

            return View(BookInvoicesOut);
        }


        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["InvoicesOutId"] = new SelectList(_context.InvoicesOuts, "InvoicesOutId", "InvoicesOutId");
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            return View();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("InvoicesOutId,BookId,Quantity")] BookInvoicesOut bookInvoicesOut)
        {
            if (ModelState.IsValid)
            {
                var insert = _context.BookInvoicesOuts.FirstOrDefault(e => e.InvoicesOutId == bookInvoicesOut.InvoicesOutId && e.BookId == bookInvoicesOut.BookId);
                if(insert == null)
                {
                    using (_context)
                    {
                        var sql = "INSERT INTO book_invoices_out (invoices_out_id, book_id,quantity) VALUES ({0},{1},{2})";
                        await _context.Database.ExecuteSqlRawAsync(sql,bookInvoicesOut.InvoicesOutId,bookInvoicesOut.BookId,bookInvoicesOut.Quantity);
                    }
                }
                else
                {
                    ViewData["InvoicesOutId"] = new SelectList(_context.InvoicesOuts, "InvoicesOutId", "InvoicesOutId", bookInvoicesOut.InvoicesOutId);
                    ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesOut.BookId);
                    return View("create", bookInvoicesOut);
                } 
                return RedirectToAction(nameof(Index));
            }
            ViewData["InvoicesOutId"] = new SelectList(_context.InvoicesOuts, "InvoicesOutId", "InvoicesOutId", bookInvoicesOut.InvoicesOutId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesOut.BookId);
            return View("create", bookInvoicesOut);
        }


        [Route("Edit/{invoicesOutId}/{bookId}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? invoicesOutId, int? bookId)
        {
            if (invoicesOutId == null || bookId == null || _context.BookInvoicesOuts == null)
            {
                return NotFound();
            }
            var bookInvoicesOut =await (from bi in _context.BookInvoicesOuts
                    where bi.BookId == bookId && bi.InvoicesOutId == invoicesOutId
                    select bi).FirstOrDefaultAsync();
            if (bookInvoicesOut == null)
            {
                return NotFound();
            }
            ViewData["InvoicesOutId"] = new SelectList(_context.InvoicesOuts, "InvoicesOutId", "InvoicesOutId", bookInvoicesOut.InvoicesOutId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesOut.BookId);
            return View(bookInvoicesOut);
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("InvoicesOutId,BookId,Quantity")] BookInvoicesOut bookInvoicesOut)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (_context)
                    {
                        var sql = "update book_invoices_out set quantity = {0} where invoices_out_id = {1} and book_id = {2}";
                        await _context.Database.ExecuteSqlRawAsync(sql, bookInvoicesOut.Quantity, bookInvoicesOut.InvoicesOutId, bookInvoicesOut.BookId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookInvoicesOutExists(bookInvoicesOut.InvoicesOutId,bookInvoicesOut.BookId))
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
            ViewData["InvoicesOutId"] = new SelectList(_context.InvoicesOuts, "InvoicesOutId", "InvoicesOutId", bookInvoicesOut.InvoicesOutId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesOut.BookId);
            return View("edit", bookInvoicesOut);
        }


        [Route("Delete/{invoicesOutId}/{bookId}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? invoicesOutId, int? bookId)
        {
            if (invoicesOutId == null || bookId == null || _context.BookInvoicesOuts == null)
            {
                return NotFound();
            }

            var bookInvoicesOut = await _context.BookInvoicesOuts
                .Include(i => i.InvoicesOut)
                .Include(i => i.Book)
                .FirstOrDefaultAsync(m => m.InvoicesOutId == invoicesOutId && m.BookId == bookId);
            if (bookInvoicesOut == null)
            {
                return NotFound();
            }

            return View(bookInvoicesOut);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("InvoicesOutId,BookId")] BookInvoicesOut bookInvoicesOut)
        {
            if (_context.BookInvoicesOuts == null)
            {
                return Problem("Entity set 'BookstoreContext.InvoicesOuts'  is null.");
            }
            if (bookInvoicesOut != null)
            {
                using (_context)
                {
                    var sql = "Delete from book_invoices_out where invoices_out_id = {0} and book_id = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, bookInvoicesOut.InvoicesOutId, bookInvoicesOut.BookId);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookInvoicesOutExists(int invoicesOutId, int bookId)
        {
            return (_context.BookInvoicesOuts?.Any(e => e.InvoicesOutId == invoicesOutId && e.BookId == bookId)).GetValueOrDefault();
        }
    }
}
