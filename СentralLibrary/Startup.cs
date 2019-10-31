using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(СentralLibrary.Startup))]
namespace СentralLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
