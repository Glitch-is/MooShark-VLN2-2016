using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MooShark2;
namespace MooShark2.Attributes
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IsAdminAttribute : AuthorizeAttribute
    {
        private Utilites.AccountHelper AccountHelper = new Utilites.AccountHelper();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session == null)
            {
                return false;
            }
            var session = httpContext.Session["User"];
            
            if(session == null)
                return false;

            var curUser = AccountHelper.getCurrentUser(session.ToString());
            if (curUser != null && curUser.roleId == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IsTeacherAttribute : AuthorizeAttribute
    {
        private Utilites.AccountHelper AccountHelper = new Utilites.AccountHelper();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session == null)
            {
                return false;
            }
            var session = httpContext.Session["User"];
            if(session == null)
                return false;

            var curUser = AccountHelper.getCurrentUser(session.ToString());
            if (curUser != null && curUser.roleId >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IsStudentAttribute : AuthorizeAttribute
    {
        private Utilites.AccountHelper AccountHelper = new Utilites.AccountHelper();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session == null)
            {
                return false;
            }
            var session = httpContext.Session["User"]; 
            if(session == null)
                return false;

            var curUser = AccountHelper.getCurrentUser(session.ToString());
            if (curUser != null && curUser.roleId >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}