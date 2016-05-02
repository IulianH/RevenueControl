using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace RevenueControl.Resources
{
    public static class Localization
    {
        static ResourceManager resourceManager;
        static readonly string defaaultCulture = "en-US";
        static Localization()
        {
            resourceManager = new ResourceManager("RevenueControl.Resources.Resources", Assembly.GetExecutingAssembly());
        }
        public static string GetResource(string key, CultureInfo culture)
        {
            
            string returnValue;
            if (culture.ToString() != defaaultCulture)
            {
                returnValue = (string)resourceManager.GetObject(key, culture);
            }
            else
            {
                returnValue = (string)resourceManager.GetObject(key);
            }
            return returnValue; 
        }
    }
}
