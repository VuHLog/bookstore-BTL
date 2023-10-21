using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public double? Salary { get; set; }

    public int? BookshelfId { get; set; }

    public int? ManagerId { get; set; }

    public virtual Bookshelf? Bookshelf { get; set; }

    public virtual ICollection<InvoicesIn> InvoicesIns { get; set; } = new List<InvoicesIn>();

    public virtual ICollection<InvoicesOut> InvoicesOuts { get; set; } = new List<InvoicesOut>();

    public virtual Manager? Manager { get; set; }
}
