using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class BookInvoicesIn
{
    public int InvoicesInId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public virtual Book? Book { get; set; } = null!;

    public virtual InvoicesIn? InvoicesIn { get; set; } = null!;
}
