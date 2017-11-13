using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EndToEnd.Models
{
    public class ControlDTO
    {
        public int ControlID { get; set; }
      
        public string ControlName { get; set; }
       
        public string CustomerName { get; set; }
        public string Description { get; set; }
        [DisplayName("Datasource Type")]
        public string ControlType { get; set; }
        [DisplayName("Created Date")]
        public System.DateTime CreatedDate { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        [DisplayName("Assigned To")]
        public Nullable<int> AssignedTo { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public IEnumerable<ControlDTO> ControlData { get; set; }
    }

}