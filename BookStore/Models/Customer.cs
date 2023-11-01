using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập địa chỉ!")]
    public string? Address { get; set; }


    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên!")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải chọn giới tính!")]
    public string? Gender { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập ngày sinh!")]
    public DateTime? DateOfBirth { get; set; }


    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập ID tài khoản!")]
    public int? UserId { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<InvoicesOut> InvoicesOuts { get; set; } = new List<InvoicesOut>();
}
