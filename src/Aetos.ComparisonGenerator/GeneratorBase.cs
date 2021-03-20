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
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    internal partial class GeneratorBase : SimpleGeneratorBase
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {

    var infoVer =
        typeof(GeneratorBase).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            this.Write("/*\r\n<auto-generated>\r\nThis file was generated by ComparisonGenerator (https://git" +
                    "hub.com/aetos382/ComparisonGenerator).\r\nDO NOT EDIT THIS FILE MANUALLY.\r\n");

    if (!string.IsNullOrEmpty(infoVer))
    {

            this.Write("\r\nGenerator version: ");
            this.Write(this.ToStringHelper.ToStringWithCulture(infoVer));
            this.Write(".\r\n");

    }

            this.Write("</auto-generated>\r\n*/\r\n\r\n");

    var nullableContext = this.SourceTypeInfo.NullableContext;

    bool annotationsEnabled = nullableContext.AnnotationsEnabled();
    bool warningsEnabled = nullableContext.WarningsEnabled();

    var pragmaValue = (annotationsEnabled, warningsEnabled) switch {
        (true, true) => "enable",
        (true, false) => "enable annotations",
        (false, true) => "enable warnings",
        _ => null
    };

    if (pragmaValue is not null)
    {

            this.Write("#nullable ");
            this.Write(this.ToStringHelper.ToStringWithCulture(pragmaValue));
            this.Write("\r\n");

    }

            this.Write("\r\nusing System;\r\n");

this.WriteUsings();

            this.Write("\r\n");

var sourceTypeInfo = this.SourceTypeInfo;

var namespaceName = sourceTypeInfo.NamespaceName;
bool hasNamespace = !string.IsNullOrEmpty(namespaceName);
if (hasNamespace)
{

            this.Write("namespace ");
            this.Write(this.ToStringHelper.ToStringWithCulture(namespaceName));
            this.Write("\r\n{\r\n");

    this.PushIndent();
}

var enclosingTypes = sourceTypeInfo.EnclosingTypes;
int numEnclosingTypes = enclosingTypes.Count;

for (int i = 0; i < numEnclosingTypes; ++i)
{
    var type = enclosingTypes[i];
    string typeKind = GetTypeKind(type);

            this.Write("partial ");
            this.Write(this.ToStringHelper.ToStringWithCulture(typeKind));
            this.Write(" ");
            this.Write(this.ToStringHelper.ToStringWithCulture(type.Name));
            this.Write("\r\n{\r\n");

    this.PushIndent();
}

this.WriteCode();

for (int i = 0; i < numEnclosingTypes; ++i)
{
    this.PopIndent();

            this.Write("}\r\n");

}


// end namespace
if (hasNamespace)
{
    this.PopIndent();

            this.Write("}\r\n");

}

            return this.GenerationEnvironment.ToString();
        }
    }
}
