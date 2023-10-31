using BookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    public class CartDTO
    {
        public int BookId { get; set; }

        public double? Cost { get; set; }

        public string? Name { get; set; }

        public int? Number { get; set; }

        public string? imageUrl { get; set; }

        public void setCart(Book book)
        {
            BookId = book.BookId;
            Cost = book.Cost;
            Name = book.Name;
            Number = book.Number;
            imageUrl = book.imageUrl;
        }
    }
}
