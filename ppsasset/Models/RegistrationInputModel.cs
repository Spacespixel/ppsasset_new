using System.ComponentModel.DataAnnotations;

namespace PPSAsset.Models
{
    public class RegistrationInputModel
    {
        [Required]
        public string ProjectID { get; set; } = string.Empty;

        [Required]
        public string ProjectName { get; set; } = string.Empty;

        [Display(Name = "เขตที่ทำงาน")]
        public string? District { get; set; }

        [Display(Name = "เขตที่อยู่ปัจจุบัน")]
        public string? Province { get; set; }

        [Required]
        [Display(Name = "ชื่อ")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "นามสกุล")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "อีเมล")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "หมายเลขโทรศัพท์")]
        public string TelNo { get; set; } = string.Empty;



        [Display(Name = "งบประมาณ")]
        public string? Budget { get; set; }

        [Display(Name = "วันที่ติดต่อกลับ")]
        [DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }

        [Display(Name = "เวลาที่ติดต่อกลับ")]
        public string? AppointmentTime { get; set; }

        public string? Remark { get; set; }

        public string? ClientFrom { get; set; }

        public bool ConsentMarketing { get; set; } = false;

        public string? RecaptchaToken { get; set; }

        public string? UtmSource { get; set; }
        public string? UtmMedium { get; set; }
        public string? UtmCampaign { get; set; }
        public string? UtmTerm { get; set; }
        public string? UtmContent { get; set; }
    }
}
