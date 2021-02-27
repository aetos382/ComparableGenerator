﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 16.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ComparableGenerator
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class EquatableOperatorGenerator : GeneratorBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {

base.TransformText();

            return this.GenerationEnvironment.ToString();
        }

protected override void WriteCode()
{
    var context = this.Context;
    var type = context.Type;

    string typeName = type.Name;
    string typeKind = GetTypeKind(type);

    var sourceType = context.SourceType;
    var options = context.Options;

    string nullableTypeName = context.NullableTypeName;

    bool hasEquals =
        sourceType.OverridesObjectEquals ||
            options.GenerateObjectEquals ||
            sourceType.IsEquatable ||
            options.GenerateEquatable;

        bool hasCompareTo =
            sourceType.IsGenericComparable ||
                options.GenerateGenericComparable ||
                sourceType.IsNonGenericComparable ||
                options.GenerateNonGenericComparable;

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write("\r\n{\r\n    public static bool operator ==(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" right)\r\n    {\r\n        if (object.ReferenceEquals(left, right))\r\n        {\r\n    " +
        "        return true;\r\n        }\r\n\r\n        if (left is null || right is null)\r\n " +
        "       {\r\n            return false;\r\n        }\r\n\r\n");


        if (hasEquals)
        {

this.Write("        return left.Equals(right);\r\n");


        }
        else if (hasCompareTo)
        {

this.Write("        return left.CompareTo(right) == 0;\r\n");


        }
        else
        {
            foreach (var member in context.Members)
            {
                string memberName = member.Name;

this.Write("        if (!EqualityComparer<");

this.Write(this.ToStringHelper.ToStringWithCulture(member.TypeName));

this.Write(">.Default.Equals(left.");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", right.");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write("))\r\n        {\r\n            return false;\r\n        }\r\n");


            }

this.Write("        return true;\r\n");


        }

this.Write("    }\r\n\r\n    public static bool operator !=(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(nullableTypeName));

this.Write(" right)\r\n    {\r\n        return !(left == right);\r\n    }\r\n}\r\n");


}

    }
}
