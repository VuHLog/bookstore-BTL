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
    [Route("Admin/BookInvoicesIn")]
    public class BookInvoicesInController : Controller
    {
        private readonly BookstoreContext _context;

        public BookInvoicesInController(BookstoreContext context)
        {
            _context = context;
        }
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.InvoicesInIdSortParam = String.IsNullOrEmpty(sortOrder) ? "invoicesinid_desc" : "";
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

            var bookInvoicesIns = from i in _context.BookInvoicesIns.Include(i => i.InvoicesIn).Include(i => i.Book)
                                   select i;

            //filter
            ViewBag.CurrentFilter = searchString;
            ViewBag.pageSize = pageSize == null ? 5 : pageSize;

            if (!String.IsNullOrEmpty(searchString))
            {
                bookInvoicesIns = bookInvoicesIns.Where(i => i.InvoicesInId.ToString().Contains(searchString)
                                                || i.Book.Name.Contains(searchString)
                                                || i.Quantity.ToString().Contains(searchString)
                                       );
            }

            switch (sortOrder)
            {
                case "invoicesinid_desc":
                    bookInvoicesIns = bookInvoicesIns.OrderByDescending(m => m.InvoicesInId);
                    break;
                case "BookId":
                    bookInvoicesIns = bookInvoicesIns.OrderBy(m => m.BookId);
                    break;
                case "bookid_desc":
                    bookInvoicesIns = bookInvoicesIns.OrderByDescending(m => m.BookId);
                    break;
                case "BookName":
                    bookInvoicesIns = bookInvoicesIns.OrderBy(m => m.Book.Name);
                    break;
                case "bookname_desc":
                    bookInvoicesIns = bookInvoicesIns.OrderByDescending(m => m.Book.Name);
                    break;
                case "Quantity":
                    bookInvoicesIns = bookInvoicesIns.OrderBy(m => m.Quantity);
                    break;
                case "quantity_desc":
                    bookInvoicesIns = bookInvoicesIns.OrderByDescending(m => m.Quantity);
                    break;
                default:
                    bookInvoicesIns = bookInvoicesIns.OrderBy(m => m.InvoicesInId);
                    break;
            }
            return View(await PaginatedList<BookInvoicesIn>.CreateAsync(bookInvoicesIns.AsNoTracking(), pageNumber ?? 1, pageSize ?? 5));
        }

        [Route("Details")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BookInvoicesIns == null)
            {
                return NotFound();
            }

            var bookInvoicesIns = await _context.BookInvoicesIns
                .Include(i => i.Book)
                .Include(i => i.InvoicesIn)
                .FirstOrDefaultAsync(m => m.InvoicesInId == id);
            if (bookInvoicesIns == null)
            {
                return NotFound();
            }

            return View(bookInvoicesIns);
        }
        

        [Route("Create")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public IActionResult Create()
        {
            ViewData["InvoicesInId"] = new SelectList(_context.InvoicesIns, "InvoicesInId", "InvoicesInId");
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId");
            return View();
        }

        // POST: Admin/InvoicesOut/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreatePost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> CreatePost([Bind("InvoicesInId,BookId,Quantity")] BookInvoicesIn bookInvoicesIn)
        {
            if (ModelState.IsValid)
            {
                var insert = _context.BookInvoicesIns.FirstOrDefault(e => e.InvoicesInId == bookInvoicesIn.InvoicesInId && e.BookId == bookInvoicesIn.BookId);
                if (insert == null)
                {
                    using (_context)
                    {
                        var sql = "INSERT INTO book_invoices_in (invoices_in_id, book_id,quantity) VALUES ({0},{1},{2})";
                        await _context.Database.ExecuteSqlRawAsync(sql, bookInvoicesIn.InvoicesInId, bookInvoicesIn.BookId, bookInvoicesIn.Quantity);
                    }
                }
                else
                {
                    ViewData["InvoicesInId"] = new SelectList(_context.InvoicesOuts, "InvoicesInId", "InvoicesInId", bookInvoicesIn.InvoicesInId);
                    ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesIn.BookId);
                    return View("create", bookInvoicesIn);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InvoicesInId"] = new SelectList(_context.InvoicesOuts, "InvoicesInId", "InvoicesInId", bookInvoicesIn.InvoicesInId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesIn.BookId);
            return View("create", bookInvoicesIn);
        }


        [Route("Edit/{invoicesInId}/{bookId}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Edit(int? invoicesInId, int? bookId)
        {
            if (invoicesInId == null || bookId == null || _context.BookInvoicesIns == null)
            {
                return NotFound();
            }
            var bookInvoicesIn = await (from bi in _context.BookInvoicesIns
                                   where bi.BookId == bookId && bi.InvoicesInId == invoicesInId
                                   select bi).FirstOrDefaultAsync();
            if (bookInvoicesIn == null)
            {
                return NotFound();
            }
            ViewData["InvoicesInId"] = new SelectList(_context.InvoicesOuts, "InvoicesInId", "InvoicesInId", bookInvoicesIn.InvoicesInId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesIn.BookId);
            return View(bookInvoicesIn);
        }

        // POST: Admin/InvoicesOut/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPost")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> EditPost([Bind("InvoicesInId,BookId,Quantity")] BookInvoicesIn bookInvoicesIn)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (_context)
                    {
                        var sql = "update book_invoices_in set quantity = {0} where invoices_in_id = {1} and book_id = {2}";
                        await _context.Database.ExecuteSqlRawAsync(sql, bookInvoicesIn.Quantity, bookInvoicesIn.InvoicesInId, bookInvoicesIn.BookId);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookInvoicesInExists(bookInvoicesIn.InvoicesInId, bookInvoicesIn.BookId))
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
            ViewData["InvoicesInId"] = new SelectList(_context.InvoicesOuts, "InvoicesInId", "InvoicesInId", bookInvoicesIn.InvoicesInId);
            ViewData["BookId"] = new SelectList(_context.Books, "BookId", "BookId", bookInvoicesIn.BookId);
            return View("edit", bookInvoicesIn);
        }

        [Route("Delete/{invoicesInId}/{bookId}")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> Delete(int? invoicesInId, int? bookId)
        {
            if (invoicesInId == null || bookId == null || _context.BookInvoicesIns == null)
            {
                return NotFound();
            }

            var bookInvoicesIn = await _context.BookInvoicesIns
                .Include(i => i.InvoicesIn)
                .Include(i => i.Book)
                .FirstOrDefaultAsync(m => m.InvoicesInId == invoicesInId && m.BookId == bookId);
            if (bookInvoicesIn == null)
            {
                return NotFound();
            }

            return View(bookInvoicesIn);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteConfirmed")]
        [Role("ROLE_MANAGER")]
        [Role("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteConfirmed([Bind("InvoicesInId,BookId")] BookInvoicesIn bookInvoicesIn)
        {
            if (_context.BookInvoicesIns == null)
            {
                return Problem("Entity set 'BookstoreContext.InvoicesIns'  is null.");
            }
            if (bookInvoicesIn != null)
            {
                using (_context)
                {
                    var sql = "Delete from book_invoices_in where invoices_in_id = {0} and book_id = {1}";
                    await _context.Database.ExecuteSqlRawAsync(sql, bookInvoicesIn.InvoicesInId, bookInvoicesIn.BookId);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookInvoicesInExists(int invoicesInId, int bookId)
        {
            return (_context.BookInvoicesIns?.Any(e => e.InvoicesInId == invoicesInId && e.BookId == bookId)).GetValueOrDefault();
        }
    }
}
