using Microsoft.AspNetCore.Hosting;

namespace NoSocNet.Extensions
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsHotModules(this IHostingEnvironment env)
        {
            return string.Compare(env.EnvironmentName, "devhot", true) == 0;
        }

    }
}
