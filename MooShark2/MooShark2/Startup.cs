using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MooShark2.Startup))]
namespace MooShark2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        
    }
}
