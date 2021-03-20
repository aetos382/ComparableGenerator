using System;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal abstract partial class GeneratorBase
    {
        private protected SourceTypeInfo SourceTypeInfo { get; }

        internal GeneratorBase(
            SourceTypeInfo sourceTypeInfo)
        {
            if (sourceTypeInfo is null)
            {
                throw new ArgumentNullException(nameof(sourceTypeInfo));
            }

            this.SourceTypeInfo = sourceTypeInfo;
        }

        protected static string GetTypeKind(
            ITypeSymbol type)
        {
            return type.TypeKind switch {
                TypeKind.Class => "class",
                TypeKind.Struct => "struct",
                TypeKind.Interface => "interface",
                _ => throw new NotSupportedException()
            };
        }

        protected void PushIndent()
        {
            this.PushIndent("    ");
        }

        protected abstract void WriteCode();

        protected virtual void WriteUsings()
        {
        }
    }
}
