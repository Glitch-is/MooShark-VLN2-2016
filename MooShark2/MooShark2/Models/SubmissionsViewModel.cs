using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MooShark2;

namespace MooShark2.Models
{
    public class SubmissionsViewModel
    {
        public IEnumerable<submission> submissions { get; set; }
        public Utilites.IdeOne ideOne { get; set; }
    }
}