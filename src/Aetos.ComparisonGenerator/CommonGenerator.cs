﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 16.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Aetos.ComparisonGenerator
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class CommonGenerator : GeneratorBase
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

this.Write("using System.Collections.Generic;\r\nusing System.ComponentModel;\r\n");


    }

    protected override void WriteCode()
    {
        var sourceTypeInfo = this.SourceTypeInfo;
        var type = sourceTypeInfo.TypeSymbol;

        string typeName = type.Name;
        string typeKind = GetTypeKind(type);

        var isValueType = sourceTypeInfo.IsValueType;
        var nullableAnnotationEnabled = sourceTypeInfo.NullableAnnotationsEnabled;

        var parameterTypeName = (isValueType, nullableAnnotationEnabled) switch {

            (true, _) => $"{typeName}?",
            (false, false) => typeName,
            (false, true) => $"{typeName}?"

        };

        string dotValue = isValueType ? ".Value" : "";

this.Write("partial ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));

this.Write(" ");

this.Write(this.ToStringHelper.ToStringWithCulture(typeName));

this.Write("\r\n{\r\n    [EditorBrowsable(EditorBrowsableState.Never)]\r\n    private static bool _" +
        "_EqualsCore(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(parameterTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(parameterTypeName));

this.Write(" right)\r\n    {\r\n");


        if (sourceTypeInfo.HasComparableAttribute)
        {

this.Write("        return __CompareCore(left, right) == 0;\r\n");


        }
        else
        {
            if (!isValueType)
            {

this.Write("        if (object.ReferenceEquals(left, right))\r\n        {\r\n            return t" +
        "rue;\r\n        }\r\n");


            }

this.Write("        \r\n        if (left is null || right is null)\r\n        {\r\n            retu" +
        "rn false;\r\n        }\r\n\r\n        bool result;\r\n");


            foreach (var member in sourceTypeInfo.Members)
            {
                string memberName = member.Name;

this.Write("\r\n        result = EqualityComparer<");

this.Write(this.ToStringHelper.ToStringWithCulture(member.FullTypeNameWithGlobalPrefix));

this.Write(">.Default.Equals(left");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", right");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(");\r\n        if (!result)\r\n        {\r\n            return result;\r\n        }\r\n");


            }


this.Write("\r\n        return true;\r\n");


        }

this.Write("    }\r\n\r\n    [EditorBrowsable(EditorBrowsableState.Never)]\r\n    private static in" +
        "t __CompareCore(\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(parameterTypeName));

this.Write(" left,\r\n        ");

this.Write(this.ToStringHelper.ToStringWithCulture(parameterTypeName));

this.Write(" right)\r\n    {\r\n");


        if (!isValueType)
        {

this.Write("        if (object.ReferenceEquals(left, right))\r\n        {\r\n            return 0" +
        ";\r\n        }\r\n\r\n");


        }

this.Write("        if (left is null)\r\n        {\r\n            return int.MinValue;\r\n        }" +
        "\r\n\r\n        if (right is null)\r\n        {\r\n            return int.MaxValue;\r\n   " +
        "     }\r\n\r\n        int result;\r\n");


        foreach (var member in sourceTypeInfo.Members)
        {
            string memberName = member.Name;

this.Write("\r\n        result = Comparer<");

this.Write(this.ToStringHelper.ToStringWithCulture(member.FullTypeNameWithGlobalPrefix));

this.Write(">.Default.Compare(left");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(", right");

this.Write(this.ToStringHelper.ToStringWithCulture(dotValue));

this.Write(".");

this.Write(this.ToStringHelper.ToStringWithCulture(memberName));

this.Write(");\r\n        if (result != 0)\r\n        {\r\n            return result;\r\n        }\r\n");


        }


this.Write("\r\n        return 0;\r\n    }\r\n}\r\n");


    }

    }
}
