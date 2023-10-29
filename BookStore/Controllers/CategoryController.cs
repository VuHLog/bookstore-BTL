using BookStore.Data;
using BookStore.Models;
using BookStore.Util;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BookstoreContext _context;

        public CategoryController(BookstoreContext context)
        {
            _context = context;
        }

        [Route("category/{id}")]
        public async Task<IActionResult> Index(int? id, string sortOrder, string costOrder,int? pageNumber, int? pageSize)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var kob =await (from k in _context.KindOfBooks
                      where k.KindOfBookId == id
                      select k).FirstOrDefaultAsync();
            if(kob != null)
            {
                ViewBag.KindOfBook = kob;
            }


            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PopularSortParam = "Popular";
            ViewBag.LastestSortParam = "Lastest";
            ViewBag.CostSortParam = "Cost";
            ViewBag.CostDescSortParam = "cost_desc";
            ViewBag.CurrentCostSortParam = costOrder;

            var books = from b in _context.Books
                         where b.KindOfBookId == id select b;
            //xác định tên trường sort
            switch (sortOrder)
            {
                case "Lastest":
                    books = books.OrderByDescending(b => b.Date);
                    break;
                default:
                    books = from b in _context.Books
                            where b.KindOfBookId == id
                            join bc in (
                                from bi in _context.BookInvoicesOuts
                                group bi by bi.BookId into g
                                select new { BookId = g.Key, TongSoLuongBan = g.Sum(bi => bi.Quantity) }
                                )
                            on b.BookId equals bc.BookId into leftJoin
                            from left in leftJoin.DefaultIfEmpty()
                            orderby left.TongSoLuongBan descending
                            select b;
                    break;
            }
            if( costOrder == "Cost")
            {
                books = books.OrderBy(b => b.Cost);
            }else if( costOrder == "cost_desc")
            {
                books = books.OrderByDescending(b => b.Cost);
            }



            if (books == null)
            {
                return NotFound();
            }
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize ?? 12));
        }
    }
}
