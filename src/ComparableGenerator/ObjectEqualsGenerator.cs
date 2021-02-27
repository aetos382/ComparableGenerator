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
    internal partial class ObjectEqualsGenerator : GeneratorBase
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

    bool delegateToEquatable =
        sourceType.IsEquatable || options.GenerateEquatable;

    bool delegateToGenericComparable =
        sourceType.IsGenericComparable || options.GenerateGenericComparable;

    bool delegateToNonGenericComparable =
        sourceType.IsNonGenericComparable || options.GenerateNonGenericComparable;

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write("\r\n{\r\n    public override bool Equals(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(context.NullableObjectTypeName));

this.Write(" other)\r\n    {\r\n        if (other is not ");

this.Write(this.ToStringHelper.ToStringWithCulture(type.Name));

this.Write(" other2)\r\n        {\r\n            return false;\r\n        }\r\n\r\n");


    if (delegateToEquatable)
    {

this.Write("        return this.Equals(other2);\r\n");



    }
    else if (delegateToGenericComparable || delegateToNonGenericComparable)
    {

this.Write("        return this.CompareTo(other2) == 0;\r\n");


    }
    else
    {

this.Write("        return Compare(this, other2) == 0;\r\n");


    }

this.Write("    }\r\n}\r\n");


}

    }
}
