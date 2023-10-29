using BookStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookstoreContext _context;

        public HomeController(BookstoreContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //top 5 sach ban chay
            var topSellingBooks = from b in _context.Books
                                  join bc in (
                                      from bi in _context.BookInvoicesOuts
                                      group bi by bi.BookId into g
                                      orderby g.Sum(bi => bi.Quantity) descending
                                      select new { BookId = g.Key, TongSoLuongBan = g.Sum(bi => bi.Quantity) }
                                  ).Take(5) on b.BookId equals bc.BookId
                                  select b;
            ViewBag.TopSellingBooks = await topSellingBooks.ToListAsync();

            //8 quyen sach ngau nhien
            var randomBooks = (from b in _context.Books
                               select b).OrderBy(x => Guid.NewGuid()).Take(8);
            ViewBag.RandomBooks = await randomBooks.ToListAsync();

            //số lượng tồn của các loại sách
            var quantityBookByKOBs = from b in _context.Books
                                    group b by new { b.KindOfBookId, b.KindOfBook.Name } into g
                                    select new {
                                        KOBID = g.Key.KindOfBookId,
                                        KOBName = g.Key.Name,
                                        Quantity = g.Sum(b => b.Number)
                                    };
            ViewBag.QuantityBookByKOBs = await quantityBookByKOBs.ToListAsync();
            return View();
        }
    }
}
