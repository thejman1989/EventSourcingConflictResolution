﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventStore.Serialization {
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
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EventStore.Serialization.Messages", typeof(Messages).Assembly);
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
        ///   Looks up a localized string similar to Deserializing stream to object of type &apos;{0}&apos;..
        /// </summary>
        internal static string DeserializingStream {
            get {
                return ResourceManager.GetString("DeserializingStream", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Registering type &apos;{0}&apos; as a known type..
        /// </summary>
        internal static string RegisteringKnownType {
            get {
                return ResourceManager.GetString("RegisteringKnownType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Serializing object graph of type &apos;{0}&apos;..
        /// </summary>
        internal static string SerializingGraph {
            get {
                return ResourceManager.GetString("SerializingGraph", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Objects of type &apos;{0}&apos; are considered to be an array: &apos;{1}&apos;..
        /// </summary>
        internal static string TypeIsArray {
            get {
                return ResourceManager.GetString("TypeIsArray", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The object to be serialized is of type &apos;{0}&apos;.  Using a typed serializer for the unknown type..
        /// </summary>
        internal static string UsingTypedSerializer {
            get {
                return ResourceManager.GetString("UsingTypedSerializer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The object to be serialized is of type &apos;{0}&apos;.  Using an untyped serializer for the known type..
        /// </summary>
        internal static string UsingUntypedSerializer {
            get {
                return ResourceManager.GetString("UsingUntypedSerializer", resourceCulture);
            }
        }
    }
}
