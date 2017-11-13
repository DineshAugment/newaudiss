using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EndToEnd.Models
{
    public partial class ActiveDirectoryModel
    {
        [DisplayName("Username")]
        public string UserName { get; set; }

        [DisplayName("Email ID")]
        public string EmailID { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        public string SearchText { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreatedDate { get; set; }

        public int ADD_ID { get; set; }
    }
}