using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StoryMakerV3.Startup))]
namespace StoryMakerV3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
