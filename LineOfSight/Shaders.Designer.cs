﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LineOfSight {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Shaders {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Shaders() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LineOfSight.Shaders", typeof(Shaders).Assembly);
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
        ///   Looks up a localized string similar to // Compiled shader for all platforms, uncompressed size: 69.1KB
        ///
        ///Shader &quot;Custom/LevelOutOfFOV&quot; {
        ///Properties {
        /// _MainTex (&quot;Base (RGB) Trans (A)&quot;, 2D) = &quot;white&quot; {}
        ///}
        ///SubShader { 
        /// Tags { &quot;QUEUE&quot;=&quot;Transparent&quot; &quot;IGNOREPROJECTOR&quot;=&quot;true&quot; &quot;RenderType&quot;=&quot;Transparent&quot; }
        ///
        ///
        /// // Stats for Vertex shader:
        /// //       d3d11 : 5 math
        /// //        d3d9 : 5 math
        /// //        gles : 222 math, 16 texture, 17 branch
        /// //       gles3 : 222 math, 16 texture, 17 branch
        /// //   glesdesktop : 222 math, 16 texture, 17 branch
        /// //       metal :  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string LevelOutOfFOV {
            get {
                return ResourceManager.GetString("LevelOutOfFOV", resourceCulture);
            }
        }
    }
}