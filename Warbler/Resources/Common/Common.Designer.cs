﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Warbler.Resources.Common {
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
    internal class Common {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Common() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Warbler.Resources.Common.Common", typeof(Common).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  at the end of input.
        /// </summary>
        internal static string AtInputEnd {
            get {
                return ResourceManager.GetString("AtInputEnd", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  at {0}.
        /// </summary>
        internal static string AtLocation {
            get {
                return ResourceManager.GetString("AtLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [Line {0}] Error{1}: {2}..
        /// </summary>
        internal static string ErrorLine {
            get {
                return ResourceManager.GetString("ErrorLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Interactive warbler interpreter v0.0.1 (2022).
        /// </summary>
        internal static string InterpreterInfo {
            get {
                return ResourceManager.GetString("InterpreterInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;exit&apos; to close the interpreter..
        /// </summary>
        internal static string InterpreterInfoExit {
            get {
                return ResourceManager.GetString("InterpreterInfoExit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} 
        ///[line {1}].
        /// </summary>
        internal static string RuntimeError {
            get {
                return ResourceManager.GetString("RuntimeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usage: warbler [file path].
        /// </summary>
        internal static string Usage {
            get {
                return ResourceManager.GetString("Usage", resourceCulture);
            }
        }
    }
}
