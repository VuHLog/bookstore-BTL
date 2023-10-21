using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<InvoicesIn> InvoicesIns { get; set; } = new List<InvoicesIn>();
}
