using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace contract_monthly__claim_system_prototype_part2.Models
{
    public enum ClaimStatus { Pending, Approved, Rejected}
    public class Claim
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string lecturerId { get; set; } = string.Empty;
        [Required]
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage ="Hours worked is required")]
        public double HoursWorked { get; set; }
        [Required(ErrorMessage="Hourly rate is required")]
        public double HourlyRate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string UploadedFileName { get; set; } = string.Empty;
        [Required]
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    }

}
