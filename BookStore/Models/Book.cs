using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public partial class Book
{
    public int BookId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên tác giả!")]
    public string? Authors { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập giá của cuốn sách!")]
    public double? Cost { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập ngày xuất bản cho sách!")]
    public DateTime? Date { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập tên của cuốn sách!")]
    public string? Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập giá của cuốn sách!")]
    public int? Number { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập thể loại của cuốn sách!")]
    public int? KindOfBookId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải nhập giới thiệu nội dung của cuốn sách!")]
    public string? Content { get; set; }

    public virtual KindOfBook? KindOfBook { get; set; }

    [Display(Name = "Chọn ảnh sách")]
    [NotMapped]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Phải upload ảnh cho sách!")]
    public IFormFile? productImage { get; set; }


    public string? imageUrl { get; set; }
}
