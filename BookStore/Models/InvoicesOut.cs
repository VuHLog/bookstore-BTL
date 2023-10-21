using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class InvoicesOut
{
    public int InvoicesOutId { get; set; }

    public DateTime? Date { get; set; }

    public int? CustomerId { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }
}
