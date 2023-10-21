using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class KindOfBook
{
    public int KindOfBookId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual Bookshelf? Bookshelf { get; set; }
}
