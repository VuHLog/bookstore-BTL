﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public bool? Enabled { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    [Required(ErrorMessage = "Mật khẩu bắt buộc phải được nhập")]
    public string? Password { get; set; }

    [Required(ErrorMessage ="Tên tài khoản bắt buộc phải được nhập")]
    public string? Username { get; set; }
}
