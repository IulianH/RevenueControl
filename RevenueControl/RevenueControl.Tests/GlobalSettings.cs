using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.Tests
{
    public static class GlobalSettings
    {
        public static string GetResourceFilePath(string resourceFile)
        {
            string currentAssemblyDirectoryName = Path.GetDirectoryName(
                                            Assembly.GetExecutingAssembly()
                                                    .Location
                                            );
            return Path.Combine(currentAssemblyDirectoryName, "Resources", resourceFile);
        }
    }
}
