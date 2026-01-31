using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPSAssetAdmin.Models
{
    [Table("sy_project")]
    public class SyProject
    {
        [Key]
        [Column("ProjectID")]
        [StringLength(155)]
        public string ProjectId { get; set; } = null!;

        [Column("ProjectName")]
        [StringLength(255)]
        public string? ProjectName { get; set; }

        [Column("CreatedDate")]
        public DateTime? CreatedDate { get; set; }
    }
}
