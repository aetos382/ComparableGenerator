﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aetos.ComparisonGenerator.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Aetos.ComparisonGenerator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
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
        ///   no members. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string NoMembersMessage {
            get {
                return ResourceManager.GetString("NoMembersMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   no members. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string NoMembersTitle {
            get {
                return ResourceManager.GetString("NoMembersTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0}.{1}: {2} is not comparable に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsNotComparableMessage {
            get {
                return ResourceManager.GetString("TypeIsNotComparableMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   type is not comparable. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsNotComparableTitle {
            get {
                return ResourceManager.GetString("TypeIsNotComparableTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0} is not partial type. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsNotPartialMessage {
            get {
                return ResourceManager.GetString("TypeIsNotPartialMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   type is not partial. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsNotPartialTitle {
            get {
                return ResourceManager.GetString("TypeIsNotPartialTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0} is static class. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsStaticMessage {
            get {
                return ResourceManager.GetString("TypeIsStaticMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   type is static. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TypeIsStaticTitle {
            get {
                return ResourceManager.GetString("TypeIsStaticTitle", resourceCulture);
            }
        }
    }
}