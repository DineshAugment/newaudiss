//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EndToEnd.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Control
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Control()
        {
            this.ActiveDirectoryUsers = new HashSet<ActiveDirectoryUser>();
            this.FileDetails = new HashSet<FileDetail>();
            this.Workflows = new HashSet<Workflow>();
        }

        public int ControlID { get; set; }
        [Required]
        [DisplayName("Control Name")]
        public string ControlName { get; set; }
        [Required]
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        public string Description { get; set; }
        [DisplayName("Datasource Type")]
        public string ControlType { get; set; }
        [DisplayFormat(DataFormatString = @"{0:MM\/dd\/yyyy}")]
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        [DisplayName("Datasource List")]
        public string DatasSource { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActiveDirectoryUser> ActiveDirectoryUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileDetail> FileDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workflow> Workflows { get; set; }
    }
}
