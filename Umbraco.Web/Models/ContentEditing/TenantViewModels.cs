using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TenantBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        public string Hostname { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateExpired { get; set; }

        public int ActiveCompaniesNbr { get; set; }

        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public string CustomerSource { get; set; }

        /// <summary>
        /// Người triển khai
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeAdminDisplay EmployeeAdmin { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
    }

    public class TenantDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public string CustomerSource { get; set; }

        /// <summary>
        /// Người triển khai
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public EmployeeAdminDisplay EmployeeAdmin { get; set; }
        public string Address { get; set; }
    }

    public class TenantRegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        [MinLength(4)]
        public string Hostname { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public string CustomerSource { get; set; }

        /// <summary>
        /// Người triển khai
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public string Address { get; set; }
    }

    public class TenantUpdateInfoViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string CompanyName { get; set; }

        /// <summary>
        /// Nguồn khách hàng
        /// </summary>
        public string CustomerSource { get; set; }

        /// <summary>
        /// Người triển khai
        /// </summary>
        public Guid? EmployeeId { get; set; }

        public string Address { get; set; }
    }

    public class TenantPaged
    {
        public TenantPaged()
        {
            Limit = 20;
        }

        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public DateTime? DateCreatedFrom { get; set; }

        public DateTime? DateCreatedTo { get; set; }
    }
}
