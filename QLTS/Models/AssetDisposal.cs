using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class AssetDisposal
    {
        [Key]
        public int AssetDisposalId { get; set; }

        [Required]
        [Display(Name = "Tài sản")]
        public int AssetId { get; set; }

        [Required]
        [Display(Name = "Ngày thanh lý")]
        [DataType(DataType.Date)]
        public DateTime DisposalDate { get; set; }

        [Required(ErrorMessage = "Lý do thanh lý không được để trống")]
        [StringLength(500)]
        [Display(Name = "Lý do thanh lý")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Hình thức thanh lý")]
        public string? DisposalMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá trị thanh lý")]
        public decimal? DisposalValue { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Người phê duyệt")]
        public int? ApprovedBy { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }


        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [ForeignKey("ApprovedBy")]
        public User? Approver { get; set; }
    }
}