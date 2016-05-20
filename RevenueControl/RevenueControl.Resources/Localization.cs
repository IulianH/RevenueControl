using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

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

        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }


        public static string GetDate(CultureInfo culture)
        {
            return GetResource("Date", culture);
        }

        public static string Date
        {
            get
            {
                return GetDate(CurrentCulture);
            }
        }

        public static string GetTransactionDetails(CultureInfo culture)
        {
            return GetResource("TransactionDetails", culture);
        }


        public static string TransactionDetails
        {
            get
            {
                return GetTransactionDetails(CurrentCulture);
            }
        }

        public static string GetDebit(CultureInfo culture)
        {
            return GetResource("Debit", culture);
        }

        public static string Debit
        {
            get
            {
                return GetDebit(CurrentCulture);
            }
        }

        public static string GetCredit(CultureInfo culture)
        {
            return GetResource("Credit", culture);
        }

        public static string Credit
        {
            get
            {
                return GetCredit(CurrentCulture);
            }
        }

    }
}
