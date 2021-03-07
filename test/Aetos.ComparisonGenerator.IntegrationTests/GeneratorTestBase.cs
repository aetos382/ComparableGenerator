using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;

using NUnit.Framework;

using Aetos.ComparisonGenerator.IntegrationTests.Injection;

namespace Aetos.ComparisonGenerator.IntegrationTests
{
    public abstract class GeneratorTestBase
    {
        protected static Assembly? RunGeneratorAndGenerateAssembly(
            ISourceGenerator generator,
            string source,
            out ImmutableArray<Diagnostic> diagnostics,
            CSharpParseOptions? parseOptions = null,
            AnalyzerConfigOptions? configOptions = null,
            IEnumerable<MetadataReference>? additionalReferences = null,
            CSharpCompilationOptions? compilationOptions = null,
            EmitOptions? emitOptions = null,
            bool printDiagnostics = true,
            bool makeTestFailIfError = true)
        {
            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            parseOptions ??= CSharpParseOptions.Default
                .WithLanguageVersion(LanguageVersion.CSharp9);

            configOptions ??= TestAnalyzerConfigOptions.Empty;

            compilationOptions ??= new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable);

            emitOptions ??= new EmitOptions(
                debugInformationFormat: DebugInformationFormat.PortablePdb);

            var optionsProvider = new TestAnalyzerConfigOptionsProvider(configOptions);

            var driver = CSharpGeneratorDriver.Create(
                new[] { generator },
                parseOptions: parseOptions,
                optionsProvider: optionsProvider);

            string assemblyName = Guid.NewGuid().ToString("D");

            var syntaxTree = CSharpSyntaxTree.ParseText(
                source,
                parseOptions,
                "Main.cs",
                Encoding.UTF8);

            var references = new List<MetadataReference> {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ITestHooks).Assembly.Location)
            };

            if (additionalReferences is not null)
            {
                references.AddRange(additionalReferences);
            }

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                compilationOptions);

            var driver2 = driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out _);

            var runResult = driver2.GetRunResult();

            var exceptions = runResult.Results
                .Select(x => x.Exception)
                .Where(x => x is not null)
                .Select(x => x!)
                .ToArray();

#pragma warning disable CS8509

            var exception = exceptions.Length switch {
                0 => null,
                1 => exceptions[0],
                > 1 => new AggregateException(exceptions)
            };

#pragma warning restore CS8509

            if (exception is not null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            var embeddedTexts = outputCompilation.SyntaxTrees
                .Select(x => EmbeddedText.FromSource(x.FilePath, x.GetText())).ToArray();

            using var peStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var emitResult = outputCompilation.Emit(
                peStream,
                pdbStream,
                options: emitOptions,
                embeddedTexts: embeddedTexts);

            diagnostics = emitResult.Diagnostics;

            if (printDiagnostics)
            {
                foreach (var d in diagnostics
                    .OrderByDescending(x => x.Severity))
                {
                    TestContext.WriteLine(d.ToString());
                }
            }

            if (!emitResult.Success)
            {
                if (makeTestFailIfError)
                {
                    Assert.Fail();
                }

                return null;
            }

            if (makeTestFailIfError)
            {
                if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    Assert.Fail();
                    return null;
                }
            }

            peStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            var assembly =
                AssemblyLoadContext.Default.LoadFromStream(
                peStream, pdbStream);

            return assembly;
        }

        protected static MethodInfo? GetMethod(
            Type type,
            string methodName,
            params Type[] parameterTypes)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            var method = type.GetMethod(
                methodName,
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly |
                BindingFlags.ExactBinding,
                default,
                parameterTypes,
                default);

            return method;
        }
    }
}
