using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class User
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public bool? Enabled { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }
}
