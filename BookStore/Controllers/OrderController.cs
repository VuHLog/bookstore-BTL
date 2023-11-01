using BookStore.CustomAtrribute;
using BookStore.Data;
using BookStore.DTO;
using BookStore.Models;
using BookStore.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BookStore.Controllers
{
    public class OrderController : Controller
    {

        private readonly BookstoreContext _context;

        public OrderController(BookstoreContext context)
        {
            _context = context;
        }

        [Route("/order")]
        [Role("ROLE_USER")]
        [Role("ROLE_ADMIN")]
        [Role("ROLE_MANAGER")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int? pageSize)
        {
            //lay thong tin id khach hang qua account
            var accountCookie = Request.Cookies["account"];
            AccountDTO accountDTO = JsonConvert.DeserializeObject<AccountDTO>(accountCookie);
            int customer_id =await (from user in _context.Users
                               join c in _context.Customers on user.UserId equals c.UserId
                               where user.Username == accountDTO.username
                               select c.CustomerId).FirstOrDefaultAsync();

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

            var bookInvoicesOuts = from bi in _context.BookInvoicesOuts.Include(i => i.InvoicesOut).Include(i => i.Book)
                                   join i in _context.InvoicesOuts on bi.InvoicesOutId equals i.InvoicesOutId
                                   join c in _context.Customers on i.CustomerId equals c.CustomerId
                                   where c.CustomerId == customer_id
                                   select bi;

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

        [Route("/cart/order")]
        [Role("ROLE_USER")]
        [Role("ROLE_ADMIN")]
        [Role("ROLE_MANAGER")]
        public async Task<IActionResult> order()
        {
            var existingCart = Request.Cookies["cart"];
            List<CartDTO>? cartList;

            var account = Request.Cookies["account"];
            AccountDTO accountDTO = JsonConvert.DeserializeObject<AccountDTO?>(account);

            if (!string.IsNullOrEmpty(existingCart))
            {
                // Kiểm tra xem dữ liệu trong cookie có đúng định dạng mảng JSON hay không
                try
                {
                    cartList = JsonConvert.DeserializeObject<List<CartDTO>?>(existingCart);
                }
                catch (JsonException)
                {
                    // Xử lý khi dữ liệu trong cookie không đúng định dạng
                    cartList = new List<CartDTO>();
                }
            }
            else
            {
                return new EmptyResult();
            }

            InvoicesOut invoicesOut = new InvoicesOut();
            //lay id customer dựa trên account
            int customer_id = await (from u in _context.Users
                              join c in _context.Customers on u.UserId equals c.UserId
                               where u.Username == accountDTO.username
                              select c.CustomerId).FirstOrDefaultAsync();
            double totalMoney = 0;
            foreach(CartDTO book in cartList)
            {
                totalMoney +=(double) book.Cost * book.quantity;
            }
            invoicesOut.Date = DateTime.Now;
            invoicesOut.CustomerId = customer_id;
            invoicesOut.EmployeeId = 1;
            invoicesOut.TotalMoney = totalMoney;

            _context.Add(invoicesOut);
            await _context.SaveChangesAsync();

            long invoicesId = invoicesOut.InvoicesOutId;
            var sql = "insert into book_invoices_out([invoices_out_id], [book_id], [quantity]) values({0}, {1}, {2})";
            foreach (CartDTO book in cartList)
            {
                _context.Database.ExecuteSqlRawAsync(sql, invoicesId,book.BookId,book.quantity);
            }



            Response.Cookies.Delete("cart");
            return new EmptyResult();
        }
    }
}
