using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập địa chỉ!")]
    public string? Address { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên!")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập số lương!")]
    public double? Salary { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập kệ sách nhân viên quản lý!")]
    public int? BookshelfId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập quản lý nhân viên này!")]
    public int? ManagerId { get; set; }

    public virtual Bookshelf? Bookshelf { get; set; }

    public virtual ICollection<InvoicesIn> InvoicesIns { get; set; } = new List<InvoicesIn>();

    public virtual ICollection<InvoicesOut> InvoicesOuts { get; set; } = new List<InvoicesOut>();

    public virtual Manager? Manager { get; set; }
}
