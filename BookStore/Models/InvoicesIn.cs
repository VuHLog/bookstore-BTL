using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class InvoicesIn
{
    public int InvoicesInId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập thời gian nhập hàng!")]
    public DateTime? Date { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải chọn nhân viên xác nhận nhập hàng!")]
    public int? EmployeeId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải chọn nhà xuất bản!")]
    public int? PublisherId { get; set; }

    public double? TotalMoney { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Publisher? Publisher { get; set; }
}
