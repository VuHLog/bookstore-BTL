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

        public int quantity = 1;

        public void setCart(Book book)
        {
            BookId = book.BookId;
            Cost = book.Cost;
            Name = book.Name;
            Number = book.Number;
            imageUrl = book.imageUrl;
        }

        public override bool Equals(object? obj)
        {
            return obj is CartDTO dTO &&
                   BookId == dTO.BookId &&
                   Cost == dTO.Cost &&
                   Name == dTO.Name &&
                   Number == dTO.Number &&
                   imageUrl == dTO.imageUrl;
        }
    }
}
