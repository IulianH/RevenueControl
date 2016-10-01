using System.IO;
using System.Reflection;

namespace RevenueControl.Tests
{
    public static class GlobalSettings
    {
        public static string GetResourceFilePath(string resourceFile)
        {
            var currentAssemblyDirectoryName = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly()
                    .Location
            );
            return Path.Combine(currentAssemblyDirectoryName, "Resources", resourceFile);
        }
    }
}