using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class KindOfBook
{
    public int KindOfBookId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên loại sách!")]
    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Bookshelf? Bookshelf { get; set; }
}
