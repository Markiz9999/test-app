using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Login")]
        [MinLength(MyValidationHelper.LoginMin, ErrorMessage = "The {0} length must be greater than {1}")]
        [MaxLength(MyValidationHelper.LoginMax, ErrorMessage = "The {0} length must be less than {1}")]
        [RegularExpression(@"(?i)^[a-z0-9_.]*$", ErrorMessage = "The {0} can contain letters, numbers, and symbols - underscore and period")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(MyValidationHelper.PasswordMin, ErrorMessage = "The {0} length must be greater than {1}")]
        [MaxLength(MyValidationHelper.PasswordMax, ErrorMessage = "The {0} length must be less than {1}")]
        public string Password { get; set; }
    }

    public class CustomerViewModel
    {
        public string CustomerId { get; set; }

        [Required]
        [Display(Name = "Login")]
        [MinLength(MyValidationHelper.LoginMin, ErrorMessage = "The {0} length must be greater than {1}")]
        [MaxLength(MyValidationHelper.LoginMax, ErrorMessage = "The {0} length must be less than {1}")]
        [RegularExpression(MyValidationHelper.LoginPattern, ErrorMessage = "The {0} can contain letters, numbers, and symbols - underscore and period")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(MyValidationHelper.PasswordMin, ErrorMessage = "The {0} length must be greater than {1}")]
        [MaxLength(MyValidationHelper.PasswordMax, ErrorMessage = "The {0} length must be less than {1}")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(MyValidationHelper.EmailMax, ErrorMessage = "The {0} length must be less than {1}")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First name")]
        [MaxLength(MyValidationHelper.NameMax, ErrorMessage = "The {0} length must be less than {1}")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last name")]
        [MaxLength(MyValidationHelper.NameMax, ErrorMessage = "The {0} length must be less than {1}")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phone")]
        [MaxLength(MyValidationHelper.PhoneMax, ErrorMessage = "The {0} length must be less than {1}")]
        [RegularExpression(MyValidationHelper.PhonePattern, ErrorMessage = "The {0} does not satisfy the format")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Roles")]
        [MinLength(1, ErrorMessage = "The {0} must contain at least {1} item")]
        public string[] Roles { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Created date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Creator name")]
        public string CreatorName { get; set; }

        [Display(Name = "Changed date")]
        public DateTime? ChangedDate { get; set; }

        [Display(Name = "Changer name")]
        public string ChangerName { get; set; }
    }
}
