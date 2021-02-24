using System;

using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal abstract partial class GeneratorBase
    {
        private protected ComparableGeneratorContext Context { get; }

        internal GeneratorBase(
            ComparableGeneratorContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.Context = context;
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
