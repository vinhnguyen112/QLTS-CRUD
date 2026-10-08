using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Mã tài sản không được để trống")]
        [StringLength(30)]
        [Display(Name = "Mã tài sản")]
        public string AssetCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên tài sản không được để trống")]
        [StringLength(150)]
        [Display(Name = "Tên tài sản")]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại tài sản")]
        public int CategoryId { get; set; }

        [Display(Name = "Phòng ban")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Nhân viên sử dụng")]
        public int? EmployeeId { get; set; }

        [StringLength(100)]
        [Display(Name = "Số serial")]
        public string? SerialNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Nguyên giá")]
        public decimal OriginalCost { get; set; }

        [Display(Name = "Ngày mua")]
        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Vị trí")]
        public string? Location { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "InStock";

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [ForeignKey("CategoryId")]
        [Display(Name = "Loại tài sản")]
        public Category? Category { get; set; }

        [ForeignKey("DepartmentId")]
        [Display(Name = "Phòng ban")]
        public Department? Department { get; set; }

        [ForeignKey("EmployeeId")]
        [Display(Name = "Nhân viên sử dụng")]
        public Employee? Employee { get; set; }


        public ICollection<ProcurementDetail> ProcurementDetails { get; set; }
            = new List<ProcurementDetail>();

        public ICollection<AssetAssignment> AssetAssignments { get; set; }
            = new List<AssetAssignment>();

        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
            = new List<MaintenanceRequest>();

        public ICollection<AssetDisposal> AssetDisposals { get; set; }
            = new List<AssetDisposal>();
    }
}