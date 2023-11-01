using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Bookshelf
{
    public int BookshelfId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập nơi kệ sách!")]
    public string? Address { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên kệ sách!")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập loại sách cho kệ sách!")]
    public int? KindOfBookId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual KindOfBook? KindOfBook { get; set; }

    public virtual Manager? Manager { get; set; }
}
