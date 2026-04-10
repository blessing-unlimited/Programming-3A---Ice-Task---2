using System.ComponentModel.DataAnnotations;

namespace IceTask_Two.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Client / company name")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required.")]
        [Display(Name = "Contact person")]
        [StringLength(200)]
        public string ContactPerson { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email")]
        [StringLength(256)]
        public string? Email { get; set; }

        [Display(Name = "Phone")]
        [StringLength(50)]
        [Phone]
        public string? Phone { get; set; }

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
