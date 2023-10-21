using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public double? Salary { get; set; }

    public int? BookshelfId { get; set; }

    public virtual Bookshelf? Bookshelf { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
