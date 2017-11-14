using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewAnalyzers;

namespace Standalone
{
    class SourceAnalyzer
    {
        private static readonly Type[] AnalyzerTypes =
        {
            typeof(EmptyOrMissingSummaryAnalyzer)
        };


        public Diagnostic[] GetDiagnostics(string path)
        {
            string source = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var baseCompilation = CSharpCompilation.Create(
                "analyzer-output",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.NetModule)
            );

            var analyzers = AnalyzerTypes.Select(Activator.CreateInstance)
                                         .Cast<DiagnosticAnalyzer>()
                                         .ToArray();

            var compilation = new CompilationWithAnalyzers(
                baseCompilation,
                ImmutableArray.Create(analyzers),
                new AnalyzerOptions(ImmutableArray.Create<AdditionalText>()),
                CancellationToken.None
            );

            ImmutableArray<Diagnostic> diagnostics = compilation.GetAnalyzerDiagnosticsAsync().Result;

            return diagnostics.ToArray();
        }

        public async Task<Diagnostic[]> GetDiagnosticsAsync(string path)
        {
            string source = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var baseCompilation = CSharpCompilation.Create(
                "analyzer-output",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.NetModule)
            );

            var analyzers = AnalyzerTypes.Select(Activator.CreateInstance)
                                         .Cast<DiagnosticAnalyzer>()
                                         .ToArray();

            var compilation = new CompilationWithAnalyzers(
                baseCompilation,
                ImmutableArray.Create(analyzers),
                new AnalyzerOptions(ImmutableArray.Create<AdditionalText>()),
                CancellationToken.None
            );

            ImmutableArray<Diagnostic> diagnostics = await compilation.GetAnalyzerDiagnosticsAsync();

            return diagnostics.ToArray();
        }
    }
}
