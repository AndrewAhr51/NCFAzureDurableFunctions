using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCFAzureDurableFunctions.src.Data.Entities
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        [ForeignKey("Donor")]
        public int DonorId { get; set; }

        [Required]
        [ForeignKey("CharitableOrganization")]
        public int OrganizationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(100)]
        public string DonationReference { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        public string Notes { get; set; }

        // Navigation Properties
        public virtual Donor Donor { get; set; }
        public virtual CharitableOrganization CharitableOrganization { get; set; }
    }

}