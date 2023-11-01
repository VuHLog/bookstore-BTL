using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập địa chỉ!")]
    public string? Address { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên!")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập số lương!")]
    public double? Salary { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập kệ sách quản lý!")]
    public int? BookshelfId { get; set; }

    public virtual Bookshelf? Bookshelf { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
