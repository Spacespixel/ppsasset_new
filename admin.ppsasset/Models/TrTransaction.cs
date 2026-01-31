using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPSAssetAdmin.Models
{
    [Table("tr_transaction")]
    public class TrTransaction
    {
        [Key]
        [Column("TransactoinID")] // Note: Typo in DB schema
        [StringLength(15)]
        public string Id { get; set; } = null!;
        
        [StringLength(255)]
        public string? FirstName { get; set; }
        
        [StringLength(255)]
        public string? LastName { get; set; }
        
        [Column("TelNo")]
        [StringLength(45)]
        public string? Phone { get; set; }
        
        [Column("EMail")]
        [StringLength(255)]
        public string? Email { get; set; }
        
        [Column("ProjectName")]
        [StringLength(255)]
        public string? Project { get; set; }
        
        [Column("utm_source")]
        [StringLength(255)]
        public string? UtmSource { get; set; }
        
        [Column("utm_medium")]
        [StringLength(255)]
        public string? UtmMedium { get; set; }
        
        [Column("utm_campaign")]
        [StringLength(255)]
        public string? UtmCampaign { get; set; }
        
        [Column("TransactionDate")]
        public DateTime RegisterDate { get; set; }
    }
}
