using BookStore.Data;
using BookStore.Models;
using BookStore.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly BookstoreContext _context;

        public BookController(BookstoreContext context)
        {
            _context = context;
        }

        [Route("book/{id}")]
        public async Task<IActionResult> Index(int? id)
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
            //Nhà xuất bản
            var publisher = await (from pb in _context.PublisherBooks
                            join p in _context.Publishers on pb.PublisherId equals p.PublisherId
                            where pb.BookId == id
                            select p).FirstOrDefaultAsync();
            ViewBag.Publisher = publisher;

            //sách tương tự ngẫu nhiên
            var similarBooks = (from b in _context.Books
                                where b.KindOfBookId == book.KindOfBookId && b.BookId != id
                                select b).OrderBy(x => Guid.NewGuid()).Take(6);
            ViewBag.SimilarBooks = await similarBooks.ToListAsync();

            //Thịnh hành
            var topSellingBooks = from b in _context.Books
                                  join bc in (
                                      from bi in _context.BookInvoicesOuts
                                      group bi by bi.BookId into g
                                      orderby g.Sum(bi => bi.Quantity) descending
                                      select new { BookId = g.Key, TongSoLuongBan = g.Sum(bi => bi.Quantity) }
                                  ).Take(3) on b.BookId equals bc.BookId
                                  select b;
            ViewBag.TopSellingBooks = await topSellingBooks.ToListAsync();
            string categoryUrl;
            //category
            switch (book.KindOfBook.Name)
            {
                case "Ngôn tình":
                    categoryUrl = "ngontinh";
                    break;
                case "Bách Hợp":
                    categoryUrl = "bachhop";
                    break;
                case "Light Novel":
                    categoryUrl = "lightnovel";
                    break;
                case "Sách thiếu nhi":
                    categoryUrl = "sachthieunhi";
                    break;
                case "Đam mỹ":
                    categoryUrl = "dammy";
                    break;
                case "Sách Văn Học Trong Nước":
                    categoryUrl = "sachvanhoctrongnuoc";
                    break;
                case "Truyện Tranh - Comic":
                    categoryUrl = "truyentranhcomic";
                    break;
                case "Truyện Tranh BL":
                    categoryUrl = "truyentranhbl";
                    break;
                default:
                    categoryUrl = "vanhocnuocngoai";
                    break;
            }
            ViewBag.CategoryUrl = categoryUrl;

            return View(book);
        }
        [Route("/NavbarSearch")]
        public async Task<IActionResult> SearchResults(string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
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

            //search
            if (!String.IsNullOrEmpty(searchString))
            {

                books = books.Where(b => b.Name.Contains(searchString));
            }
            return View("SearchResults",await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize ?? 4));
        }
    }
}
