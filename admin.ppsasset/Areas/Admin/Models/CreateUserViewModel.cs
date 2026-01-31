using System.ComponentModel.DataAnnotations;

namespace PPSAssetAdmin.Areas.Admin.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DisplayName { get; set; }

        [Required]
        public string Role { get; set; } = "Admin";
        
        public bool IsActive { get; set; } = true;
    }
}
