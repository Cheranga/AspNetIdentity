using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetIdentity.WebApi.Models
{
    public class CreateRoleBindingDto
    {
        [Required]
        [Display(Name = "Role Name")]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} charactors long", MinimumLength = 2)]
        public string Name { get; set; }
    }
}