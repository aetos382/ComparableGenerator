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
    internal partial class NonGenericComparableGenerator : GeneratorBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {

base.TransformText();

            return this.GenerationEnvironment.ToString();
        }

protected override void WriteUsings()
{

this.Write("using System.Collections.Generic;\r\n");


}


protected override void WriteCode()
{
    var context = this.Context;
    var type = context.Type;

    string typeName = type.Name;
    string typeKind = GetTypeKind(type);

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write(" :\r\n    IComparable\r\n{\r\n    public int CompareTo(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(context.NullableObjectTypeName));

this.Write(" other)\r\n    {\r\n        if (other is null)\r\n        {\r\n            return int.Max" +
        "Value;\r\n        }\r\n\r\n        if (other is not ");

this.Write(this.ToStringHelper.ToStringWithCulture(type.Name));

this.Write(" other2)\r\n        {\r\n            throw new ArgumentException();\r\n        }\r\n\r\n");


    if (context.SourceType.IsGenericComparable ||
        context.Options.GenerateGenericComparable)
    {

this.Write("        return this.CompareTo(other2);\r\n");


    }
    else
    {

this.Write("        int result;\r\n");


        foreach (var member in context.Members)
        {
            string memberName = member.Name;

this.Write("        result = Comparer<object>.Default.Compare(this.");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", other2.");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(");\r\n        if (result != 0)\r\n        {\r\n            return result;\r\n        }\r\n");


        }

this.Write("\r\n        return 0;\r\n");


    }

this.Write("    }\r\n}\r\n");


}

    }
}
