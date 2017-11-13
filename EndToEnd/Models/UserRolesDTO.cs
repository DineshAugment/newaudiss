using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EndToEnd.Models
{
    public class ExpandedUserDTO
    {
        [Key]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Display(Name = "Lockout End Date Utc")]
        public DateTime? LockoutEndDateUtc { get; set; }
        public int AccessFailedCount { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<UserRolesDTO> Roles { get; set; }
    }

    public class UserRolesDTO
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserRoleDTO
    {
        [Key]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class RoleDTO
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
    }

    public class UserAndRolesDTO
    {
        [Key]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public List<UserRoleDTO> colUserRoleDTO { get; set; }
    }
}