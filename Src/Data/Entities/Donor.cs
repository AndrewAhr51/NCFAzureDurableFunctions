using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCFAzureDurableFunctions.src.Data.Entities 
{
    public class Donor
    {
        [Key] // ✅ Explicitly define primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Unique Identifier

        public required int UserId { get; set; }  // Links donor to a user
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

        // ✅ Split Address into separate fields
        public required string StreetAddressLine1 { get; set; }
        public string? StreetAddressLine2 { get; set; }  // Optional second address line
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }

        public required DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Timestamp for creation
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Timestamp for updates

        // ✅ Optional: Link Donations to Donors
        public ICollection<Donation> Transactions { get; set; } = new List<Donation>();
    }
}