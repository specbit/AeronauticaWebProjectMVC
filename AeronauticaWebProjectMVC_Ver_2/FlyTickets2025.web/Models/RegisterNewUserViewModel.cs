//using System.ComponentModel.DataAnnotations;

//namespace FlyTickets2025.web.Models
//{
//    public class RegisterNewUserViewModel
//    {
//        [Required]
//        [Display(Name = "First Name")]
//        public required string FirstName { get; set; }

//        [Required]
//        [Display(Name = "Last Name")]
//        public required string LastName { get; set; }

//        [Required]
//        [DataType(DataType.EmailAddress)]
//        public required string Username { get; set; }

//        [Required]
//        [MinLength(6)]
//        public required string Password { get; set; }

//        [Required]
//        [Compare("Password")]
//        public required string Confirm { get; set; }
//    }
//}
using FlyTickets2025.web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace FlyTickets2025.web.Models
{
    public class RegisterNewUserViewModel
    {
        //[Required]
        //[Display(Name = "First Name")]
        //public required string FirstName { get; set; }

        //[Required]
        //[Display(Name = "Last Name")]
        //public required string LastName { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //public required string Username { get; set; }

        //[Required]
        //[MinLength(6)]
        //public required string Password { get; set; }

        //[Required]
        //[Compare("Password")]
        //public required string Confirm { get; set; }

        //public DocumentType DocumentType { get; set; }

        //[StringLength(50)]
        //public string DocumentNumber { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")] // Username is Email
        public required string Username { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm password")]
        public required string Confirm { get; set; }
    }
}
