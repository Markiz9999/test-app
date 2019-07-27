using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class SimpleCustomerViewModel
    {
        public string CustomerId { get; set; }

        [Required]
        [Display(Name = "Login")]
        [MinLength(MyValidationHelper.LoginMin, ErrorMessage = "The {0} length must be greater than {1}")]
        [MaxLength(MyValidationHelper.LoginMax, ErrorMessage = "The {0} length must be less than {1}")]
        [RegularExpression(@"(?i)^[a-z0-9_.]*$", ErrorMessage = "The {0} can contain letters, numbers, and symbols - underscore and period")]
        public string Login { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
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
        [RegularExpression(@"^(\s*)?(\+)?([- _():=+]?\d[- _():=+]?){10,14}(\s*)?$", ErrorMessage = "The {0} does not satisfy the format")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool Active { get; set; }
    }
}