using System.ComponentModel.DataAnnotations;

namespace QLTS.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Mã loại tài sản không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã loại tài sản")]
        public string CategoryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên loại tài sản không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên loại tài sản")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}