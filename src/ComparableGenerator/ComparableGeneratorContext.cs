using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace ComparableGenerator
{
    internal class ComparableGeneratorContext
    {
        public Compilation Compilation { get; }

        public string? Namespace { get; }

        public IReadOnlyList<INamedTypeSymbol> Types { get; }

        public IReadOnlyList<SourceMemberInfo> Members { get; }

        public GenerateOptions Options { get; }

        public CommonTypes CommonTypes { get; }

        public SourceTypeInfo SourceType { get; }

        public NullableContext NullableContext { get; }

        public bool NullableAnnotationEnabled
        {
            get
            {
                return this.NullableContext.AnnotationsEnabled();
            }
        }

        public ITypeSymbol Type
        {
            get
            {
                return this.Types.Last();
            }
        }

        public string NullableTypeName
        {
            get
            {
                if (this.NullableAnnotationEnabled)
                {
                    return $"{this.Type.Name}?";
                }
                else
                {
                    return this.Type.Name;
                }
            }
        }
        
        public string NullableObjectTypeName
        {
            get
            {
                if (this.NullableAnnotationEnabled)
                {
                    return "object?";
                }
                else
                {
                    return "object";
                }
            }
        }

        public ComparableGeneratorContext(
            Compilation compilation,
            string? namespaceName,
            IReadOnlyList<INamedTypeSymbol> types,
            IReadOnlyList<SourceMemberInfo> members,
            GenerateOptions options,
            CommonTypes commonTypes,
            SourceTypeInfo sourceTypeInfo,
            NullableContext nullableContext)
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (members is null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (commonTypes is null)
            {
                throw new ArgumentNullException(nameof(commonTypes));
            }

            if (sourceTypeInfo is null)
            {
                throw new ArgumentNullException(nameof(sourceTypeInfo));
            }

            this.Compilation = compilation;
            this.Namespace = namespaceName;
            this.Types = types;
            this.Members = members;
            this.Options = options;
            this.CommonTypes = commonTypes;
            this.SourceType= sourceTypeInfo;
            this.NullableContext = nullableContext;
        }
    }
}
