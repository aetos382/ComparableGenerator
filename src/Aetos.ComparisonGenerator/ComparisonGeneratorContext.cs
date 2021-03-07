﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace Aetos.ComparisonGenerator
{
    internal class ComparisonGeneratorContext
    {
        public Compilation Compilation { get; }

        public string? Namespace { get; }

        public IReadOnlyList<INamedTypeSymbol> Types { get; }

        public IReadOnlyList<SourceMemberInfo> Members { get; }

        public GeneratorOptions Options { get; }

        public KnownTypes CommonTypes { get; }

        public SourceTypeInfo SourceType { get; }

        public NullableContext NullableContext { get; }

        public bool IsNullable
        {
            get
            {
                return !this.Type.IsValueType || this.NullableAnnotationEnabled;
            }
        }

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
                return this.Types[this.Types.Count - 1];
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

        public ComparisonGeneratorContext(
            Compilation compilation,
            string? namespaceName,
            IReadOnlyList<INamedTypeSymbol> types,
            IReadOnlyList<SourceMemberInfo> members,
            GeneratorOptions options,
            KnownTypes commonTypes,
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
