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
    
    public partial class grades
    {
        public int id { get; set; }
        public Nullable<int> grade { get; set; }
        public int assignmentId { get; set; }
        public int teamId { get; set; }
    
        public virtual assignment assignment { get; set; }
        public virtual team team { get; set; }
    }
}
