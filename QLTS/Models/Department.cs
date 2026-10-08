using System.ComponentModel.DataAnnotations;

namespace QLTS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Mã phòng ban không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã phòng ban")]
        public string DepartmentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên phòng ban")]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();

        public ICollection<Asset> Assets { get; set; }
            = new List<Asset>();
    }
}