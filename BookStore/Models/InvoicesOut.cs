using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class InvoicesOut
{
    public int InvoicesOutId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập ngày xuất!")]
    public DateTime? Date { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập khách hàng đặt!")]
    public int? CustomerId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập nhân viên nhận đơn hàng!")]
    public int? EmployeeId { get; set; }

    public double? TotalMoney { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }
}
