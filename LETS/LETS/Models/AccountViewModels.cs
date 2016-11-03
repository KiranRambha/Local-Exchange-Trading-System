﻿using LETS.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LETS.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Please enter at least eight characters, including one letter, number and special character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotUsernameViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RegisterUserViewModel
    {
        [BsonId]
        public string PersonId { get; set; }

        public About About { get; set; }

        public Account Account { get; set; }
    }

    public class About
    {
        [Required(ErrorMessage = "What is your title.")]
        [Display(Name = "Title")]
        [CategoryLookup("RegisterUser.Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter your first name.")]
        [Display(Name = "User Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        [Display(Name = "User Name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Please select your gender.")]
        [Display(Name = "Gender")]
        [CategoryLookup("RegisterUser.Gender")]
        public string Gender { get; set; }
    }

    public class Account
    {
        [Required(ErrorMessage = "Please enter your username.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Please enter at least eight characters, including one letter, number and special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Please ensure that the password here matches the password that you provided above")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Please enter at least eight characters, including one letter, number and special character")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}