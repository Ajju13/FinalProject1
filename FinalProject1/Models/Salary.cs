//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinalProject1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Salary
    {
        public long Salary_ID { get; set; }
        public long Teacher_ID { get; set; }
        public string Month { get; set; }
        public long Gross_Salary { get; set; }
        public long Bonus { get; set; }
        public long Deductions { get; set; }
    
        public virtual Facutly Facutly { get; set; }
    }
}
