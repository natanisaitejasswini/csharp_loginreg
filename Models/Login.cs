using System;
using System.ComponentModel.DataAnnotations;

namespace aspLoginReg.Models
{
    public abstract class BaseEntity {}

    public class User : BaseEntity
        {
            public int id;
            [Required]
            [MinLength(2)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string first_name { get; set; }
            [Required]
            [MinLength(1)]
            [RegularExpression(@"^[a-zA-Z]+$")]
            public string last_name { get; set; }
            [Required]
            [EmailAddress]
            public string email{ get; set; }
            [Required]
            [MinLength(3)]
            public string password { get; set; }
            [Required]
            [Compare("password")]
            public string confirm_password {get; set;}
            public DateTime created_at;
        }
}