using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Creuna.AzureAD.EpiserverTest.Startup))]

namespace Creuna.AzureAD.EpiserverTest
{
    public partial class Startup
    {
//        public void Configuration(IAppBuilder app)
//        {
//            ConfigureAuth(app);
//        }
    }
}