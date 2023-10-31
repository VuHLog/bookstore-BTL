using BookStore.Data;
using BookStore.DTO;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly BookstoreContext _context;

        public CartController(BookstoreContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/cart/addCart/{id}")]
        public async Task<IActionResult> CartNavBar(int id)
        {
            if (_context.Books == null) return BadRequest();

            // Lấy sách từ database bằng id
            Book book = await (from b in _context.Books
                               where b.BookId == id
                               select b).FirstOrDefaultAsync();
            CartDTO cartDTO = new CartDTO();
            cartDTO.setCart(book);
            var existingCart = Request.Cookies["cart"];

            List<CartDTO>? cartList;

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
                // Cookie "cart" chưa tồn tại, tạo danh sách sách mới
                cartList = new List<CartDTO>();
            }

            // Thêm sách mới vào danh sách sách
            bool check = false;
            int index = -1;
            foreach(CartDTO cart in cartList)
            {
                index++;
                if (cart.Equals(cartDTO))
                {
                    check = true;
                    break;
                }

            }
            if (check == true)
            {
                cartList[index].quantity++;
            }
            else
            {
                cartList.Add(cartDTO);
            }

            // Lưu trữ danh sách sách dưới dạng mảng JSON trong cookie "cart"
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartList));

            return View("CartNavBar");
        }

        [Route("/cart/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingCart = Request.Cookies["cart"];

            List<CartDTO>? cartList;

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
                // Cookie "cart" chưa tồn tại, tạo danh sách sách mới
                cartList = new List<CartDTO>();
            }

            // Thêm sách mới vào danh sách sách
            foreach (CartDTO cart in cartList)
            {
                if(cart.BookId == id)
                {
                    cartList.Remove(cart);
                    break;
                }

            }

            // Lưu trữ danh sách sách dưới dạng mảng JSON trong cookie "cart"
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("cart", JsonConvert.SerializeObject(cartList));
            return new EmptyResult();
        }
    }
}
