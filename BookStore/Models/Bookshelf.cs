using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Bookshelf
{
    public int BookshelfId { get; set; }

    public string? Address { get; set; }

    public string? Name { get; set; }

    public int? KindOfBookId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual KindOfBook? KindOfBook { get; set; }

    public virtual Manager? Manager { get; set; }
}
