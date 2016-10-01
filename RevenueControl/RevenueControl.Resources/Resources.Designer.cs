﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RevenueControl.Resource {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RevenueControl.Resource.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Credit.
        /// </summary>
        public static string Credit {
            get {
                return ResourceManager.GetString("Credit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Date.
        /// </summary>
        public static string Date {
            get {
                return ResourceManager.GetString("Date", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Debit.
        /// </summary>
        public static string Debit {
            get {
                return ResourceManager.GetString("Debit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation failed. If the error perssists please contact the system administrator..
        /// </summary>
        public static string GenericError {
            get {
                return ResourceManager.GetString("GenericError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation failed. Please write down the error code {0}. If the error perssists please contact the system administrator..
        /// </summary>
        public static string GenericErrorWithErrorCode {
            get {
                return ResourceManager.GetString("GenericErrorWithErrorCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed sucessfully.
        /// </summary>
        public static string GenericSuccess {
            get {
                return ResourceManager.GetString("GenericSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transaction Details.
        /// </summary>
        public static string TransactionDetails {
            get {
                return ResourceManager.GetString("TransactionDetails", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} transactions successfully added.
        /// </summary>
        public static string TransactionsAdded {
            get {
                return ResourceManager.GetString("TransactionsAdded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} transactions successfully added, {1} transactions already existed in the database.
        /// </summary>
        public static string TransactionsAddedWithWarnings {
            get {
                return ResourceManager.GetString("TransactionsAddedWithWarnings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All the transactions you are trying to add already exist in the database.
        /// </summary>
        public static string ZeroTransactionsAdded {
            get {
                return ResourceManager.GetString("ZeroTransactionsAdded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file doesn&apos;t contain any transaction.
        /// </summary>
        public static string ZeroTransactionsInFile {
            get {
                return ResourceManager.GetString("ZeroTransactionsInFile", resourceCulture);
            }
        }
    }
}
