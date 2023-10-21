using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class InvoicesIn
{
    public int InvoicesInId { get; set; }

    public DateTime? Date { get; set; }

    public int? EmployeeId { get; set; }

    public int? PublisherId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Publisher? Publisher { get; set; }
}
