using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class BookInvoicesOut
{
    public int InvoicesOutId { get; set; }

    public int BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual InvoicesOut InvoicesOut { get; set; } = null!;
}
