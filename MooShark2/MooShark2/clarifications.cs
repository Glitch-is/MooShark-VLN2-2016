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
    
    public partial class clarifications
    {
        public int id { get; set; }
        public int clarificationId { get; set; }
        public int problemId { get; set; }
    
        public virtual clarification clarification { get; set; }
        public virtual problem problem { get; set; }
    }
}
