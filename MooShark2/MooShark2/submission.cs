//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MooShark2
{
    using System;
    using System.Collections.Generic;
    
    public partial class submission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public submission()
        {
            this.submissions = new HashSet<submissions>();
            this.testcaseOutput = new HashSet<testcaseOutput>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        public int problemId { get; set; }
        public Nullable<System.DateTime> submitted { get; set; }
        public string status { get; set; }
        public Nullable<int> languageId { get; set; }
        public int correctTestcases { get; set; }
    
        public virtual problem problem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<submissions> submissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<testcaseOutput> testcaseOutput { get; set; }
    }
}
