using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspNet_FilRouge.Startup))]
namespace AspNet_FilRouge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
