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
    
    public partial class FileDetail
    {
        public System.Guid ID { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int ControlId { get; set; }
    
        public virtual Control Control { get; set; }
    }
}