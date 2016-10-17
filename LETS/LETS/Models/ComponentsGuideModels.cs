using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LETS.Models
{
    public class ComponentsGuideModels
    {
        [Required(ErrorMessage = "What is your title.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "First Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter you email address.")]
        [EmailAddress(ErrorMessage = "You have entered an invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"(?=.*[A-Z,a-z])(?=.*[1-9])(?=.*[!-/,:-@,[-`,{-~,\243])([!-~,\243]){8,}", ErrorMessage = "Please enter at least eight characters, including one letter, number and special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Please ensure that the password here matches the password that you provided above")]
        [RegularExpression(@"(?=.*[A-Z,a-z])(?=.*[1-9])(?=.*[!-/,:-@,[-`,{-~,\243])([!-~,\243]){8,}", ErrorMessage = "Please enter at least eight characters, including one letter, number and special character")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Display(Name = "Phone Number")]
        public int? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please check to agree.")]
        public bool Agree { get; set; }
    }
}