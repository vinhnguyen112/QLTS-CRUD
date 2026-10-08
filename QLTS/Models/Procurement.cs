using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class Procurement
    {
        [Key]
        public int ProcurementId { get; set; }

        [Required(ErrorMessage = "Mã phiếu nhập không được để trống")]
        [StringLength(30)]
        [Display(Name = "Mã phiếu nhập")]
        public string ProcurementCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Người đề nghị không được để trống")]
        [StringLength(100)]
        [Display(Name = "Người đề nghị")]
        public string RequesterName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phòng ban")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Ngày đề nghị")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do")]
        public string? Reason { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Chi phí dự kiến")]
        public decimal PlannedCost { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("DepartmentId")]
        [Display(Name = "Phòng ban")]
        public Department? Department { get; set; }

        public ICollection<ProcurementDetail> ProcurementDetails { get; set; }
            = new List<ProcurementDetail>();
    }
}