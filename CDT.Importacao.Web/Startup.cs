using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CDT.Importacao.Web.Startup))]
namespace CDT.Importacao.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
