using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class AssetAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        [Display(Name = "Tài sản")]
        public int AssetId { get; set; }

        [Display(Name = "Phòng ban cũ")]
        public int? FromDepartmentId { get; set; }

        [Display(Name = "Phòng ban mới")]
        public int? ToDepartmentId { get; set; }

        [Display(Name = "Nhân viên cũ")]
        public int? FromEmployeeId { get; set; }

        [Display(Name = "Nhân viên mới")]
        public int? ToEmployeeId { get; set; }

        [Required]
        [Display(Name = "Ngày cấp phát")]
        [DataType(DataType.Date)]
        public DateTime AssignmentDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string? Reason { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Completed";

        [Required]
        [Display(Name = "Người thực hiện")]
        public int CreatedBy { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [ForeignKey("FromDepartmentId")]
        public Department? FromDepartment { get; set; }

        [ForeignKey("ToDepartmentId")]
        public Department? ToDepartment { get; set; }

        [ForeignKey("FromEmployeeId")]
        public Employee? FromEmployee { get; set; }

        [ForeignKey("ToEmployeeId")]
        public Employee? ToEmployee { get; set; }

        [ForeignKey("CreatedBy")]
        public User? Creator { get; set; }
    }
}