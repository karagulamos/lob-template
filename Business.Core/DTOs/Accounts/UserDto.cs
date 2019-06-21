using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Business.Core.Common.Enums;

namespace Business.Core.DTOs.Accounts
{
    public class UserDto
    {
        public UserDto()
        {
            Roles = new List<string>();
        }

        public long UserId { get; set; }

        public bool IsActive { get; set; }

        public UserType UserType { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please specify a valid email.")]
        [EmailAddress(ErrorMessage = "Please specify a valid email.")]
        public string Email { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public List<string> Roles { get; set; }
    }
}
