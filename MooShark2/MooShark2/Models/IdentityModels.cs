using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MooShark2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MooShark2.testcase> testcases1 { get; set; }

        public System.Data.Entity.DbSet<MooShark2.assignment> assignments { get; set; }

        public System.Data.Entity.DbSet<MooShark2.clarification> clarifications1 { get; set; }

        public System.Data.Entity.DbSet<MooShark2.groupMembers> groupMembers { get; set; }

        public System.Data.Entity.DbSet<MooShark2.team> teams1 { get; set; }

        public System.Data.Entity.DbSet<MooShark2.problem> problems1 { get; set; }

        public System.Data.Entity.DbSet<MooShark2.submission> submissions1 { get; set; }

        public System.Data.Entity.DbSet<MooShark2.testcaseOutput> testcaseOutput { get; set; }
    }
}