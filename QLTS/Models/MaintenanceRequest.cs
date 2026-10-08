using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int MaintenanceRequestId { get; set; }

        [Required]
        [Display(Name = "Tài sản")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Người yêu cầu")]
        public int RequesterId { get; set; }

        [Display(Name = "Kỹ thuật viên")]
        public int? TechnicianId { get; set; }

        [Display(Name = "Ngày yêu cầu")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Nội dung sự cố không được để trống")]
        [StringLength(500)]
        [Display(Name = "Mô tả sự cố")]
        public string ProblemDescription { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Giải pháp xử lý")]
        public string? Solution { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Ngày hoàn thành")]
        public DateTime? CompletedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Chi phí")]
        public decimal? Cost { get; set; }


        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [ForeignKey("RequesterId")]
        public User? Requester { get; set; }

        [ForeignKey("TechnicianId")]
        public User? Technician { get; set; }
    }
}