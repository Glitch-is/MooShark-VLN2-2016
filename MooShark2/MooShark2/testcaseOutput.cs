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
    
    public partial class testcaseOutput
    {
        public int id { get; set; }
        public int submissionId { get; set; }
        public int testcaseId { get; set; }
        public Nullable<int> ideoneId { get; set; }
        public string output { get; set; }
        public Nullable<int> statusId { get; set; }
        public Nullable<int> resultId { get; set; }
        public string stderr { get; set; }
        public string time { get; set; }
        public string status { get; set; }
    
        public virtual submission submission { get; set; }
        public virtual testcase testcase { get; set; }
    }
}
