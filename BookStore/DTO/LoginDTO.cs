using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Mật khẩu bắt buộc phải được nhập")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Tên tài khoản bắt buộc phải được nhập")]
        public string? Username { get; set; }
    }
}
