using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public partial class User
{
    public int UserId { get; set; }

    [EmailAddress(ErrorMessage ="Email không hợp lệ!")]
    [Required(ErrorMessage = "Email bắt buộc phải được nhập")]
    public string? Email { get; set; }

    public bool? Enabled { get; set; }

    [Required(ErrorMessage = "Phải nhập tên")]
    public string? Firstname { get; set; }

    [Required(ErrorMessage = "Phải nhập họ")]
    public string? Lastname { get; set; }

    [Required(ErrorMessage = "Mật khẩu bắt buộc phải được nhập")]
    public string? Password { get; set; }

    [Required(ErrorMessage ="Tên tài khoản bắt buộc phải được nhập")]
    public string? Username { get; set; }

    [Display(Name="Chọn ảnh đại diện cho tài khoản")]
    [NotMapped]
    public IFormFile? avatar { get; set; }

    public string? avatarUrl { get; set; }

    public virtual Customer? Customer { get; set; }
}
