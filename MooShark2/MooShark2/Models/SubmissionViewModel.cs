using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MooShark2;

namespace MooShark2.Models
{
    public class SubmissionViewModel
    {
        public submission submission { get; set; }
        public IEnumerable<testcaseOutput> testcases { get; set; }
        public Utilites.IdeOne ideOne { get; set; }
    }

    public class SubmissionTeacherViewModel
    {
        public List<SubmissionTeacherExtraClass> TableData { get; set; }
    }

    public class SubmissionTeacherExtraClass
    {
        public string teamName { get; set; }
        public int submissionId { get; set; }
    }
}