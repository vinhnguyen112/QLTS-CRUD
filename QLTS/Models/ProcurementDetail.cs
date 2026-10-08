using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTS.Models
{
    public class ProcurementDetail
    {
        [Key]
        public int ProcurementDetailId { get; set; }

        [Required]
        [Display(Name = "Phiếu nhập")]
        public int ProcurementId { get; set; }

        [Required]
        [Display(Name = "Tài sản")]
        public int AssetId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal UnitCost { get; set; }

        [ForeignKey("ProcurementId")]
        public Procurement? Procurement { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }
    }
}