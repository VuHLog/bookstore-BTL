﻿using BookStore.Data;
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
            //category
            ViewBag.KindOfBookId = book.KindOfBook.KindOfBookId;

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

                books = books.Where(b => b.Name.Contains(searchString)).Take(2);
            }
            return View("SearchResults",await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize ?? 4));
        }

        [Route("/SearchResultList")]
        public async Task<IActionResult> SearchResultList(int? id,string searchString, string currentFilter, string sortOrder, string costOrder, int? pageNumber, int? pageSize)
        {
            //sort
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PopularSortParam = "Popular";
            ViewBag.LastestSortParam = "Lastest";
            ViewBag.CostSortParam = "Cost";
            ViewBag.CostDescSortParam = "cost_desc";
            ViewBag.CurrentCostSortParam = costOrder;

            var books = from b in _context.Books
                        select b;

            //luu filter hien tai
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //xác định tên trường sort
            switch (sortOrder)
            {
                case "Lastest":
                    books = books.OrderByDescending(b => b.Date);
                    break;
                default:
                    books = from b in _context.Books
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
            if (costOrder == "Cost")
            {
                books = books.OrderBy(b => b.Cost);
            }
            else if (costOrder == "cost_desc")
            {
                books = books.OrderByDescending(b => b.Cost);
            }

            if (!String.IsNullOrEmpty(searchString))
            {

                books = books.Where(b =>b.Name.Contains(searchString)
                                       );
            }

            if (books == null)
            {
                return NotFound();
            }
            return View(await PaginatedList<Book>.CreateAsync(books.AsNoTracking(), pageNumber ?? 1, pageSize ?? 3));
        }
    }
}
