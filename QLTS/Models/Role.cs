using System.ComponentModel.DataAnnotations;

namespace QLTS.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên vai trò")]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}