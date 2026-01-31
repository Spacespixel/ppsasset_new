using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPSAssetAdmin.Models
{
    [Table("sy_user")]
    public class SyUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Role { get; set; } = "Admin";
        
        public DateTime? LastLogin { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
