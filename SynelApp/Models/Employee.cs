using System.ComponentModel.DataAnnotations;

namespace SynelApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string PayrollNumber { get; set; } = "";

        [Required]
        [MaxLength(50)] 
        public string Forenames { get; set; } = "";

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telephone { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = "";

        [MaxLength(255)]
        public string? Address2 { get; set; }

        [Required]
        [MaxLength(20)]
        public string Postcode { get; set; } = "";

        [Required ]
        [EmailAddress]
        [MaxLength(100)]
        public string EmailHome { get; set; } = "";

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}
