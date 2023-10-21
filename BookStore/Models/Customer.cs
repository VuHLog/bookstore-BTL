using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<InvoicesOut> InvoicesOuts { get; set; } = new List<InvoicesOut>();
}
