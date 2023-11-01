using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class UsersRole
{
    public int UserId { get; set; }

    public long RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
