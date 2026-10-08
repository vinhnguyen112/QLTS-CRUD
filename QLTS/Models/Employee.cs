using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        [StringLength(20)]
        [Display(Name = "Mã nhân viên")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phòng ban")]
        public int DepartmentId { get; set; }

        [StringLength(100)]
        [Display(Name = "Chức vụ")]
        public string? Position { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [ForeignKey("DepartmentId")]
        [Display(Name = "Phòng ban")]
        public Department? Department { get; set; }

        public ICollection<Asset> Assets { get; set; }
            = new List<Asset>();

        public ICollection<AssetAssignment> AssetAssignments { get; set; }
            = new List<AssetAssignment>();
    }
}