using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string? Authors { get; set; }

    public double? Cost { get; set; }

    public DateTime? Date { get; set; }

    public string? Name { get; set; }

    public int? Number { get; set; }

    public int? KindOfBookId { get; set; }

    public string? Content { get; set; }

    public virtual KindOfBook? KindOfBook { get; set; }

    [Display(Name = "Chọn ảnh sách")]
    [NotMapped]
    public IFormFile? productImage { get; set; }


    public string? imageUrl { get; set; }
}
