using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace JKMP.Core.Analyzers.Tests;

public class CSharpVerifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static Task VerifyAnalyzer(string source, params DiagnosticResult[] diagnostics) => VerifyAnalyzer(new[] { source }, diagnostics);

    public static Task VerifyAnalyzer(string[] sources, params DiagnosticResult[] diagnostics)
    {
        var test = new TestJKMP();

        foreach (var source in sources)
        {
            test.TestState.Sources.Add(source);
        }

        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);
        return test.RunAsync();
    }

    public class TestJKMP : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, MSTestVerifier>
    {
        // Setting default project name will also set the assembly name to the same name
        // This is needed due to how Core looks for plugin classes
        protected override string DefaultTestProjectName => "JKMP.Plugin.Test";

        public TestJKMP()
        {
            ImmutableArray<PackageIdentity> packages = ImmutableArray.Create(
                new PackageIdentity("JKMP.Core", Constants.CoreVersion)
            );

            ReferenceAssemblies = ReferenceAssemblies.Default
                .AddPackages(packages);
        }

        protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
        {
            TAnalyzer analyzer = new();

            foreach (var provider in CodeFixProviderDiscovery.GetCodeFixProviders(Language))
            {
                if (analyzer.SupportedDiagnostics.Any(diagnostic => provider.FixableDiagnosticIds.Contains(diagnostic.Id)))
                {
                    yield return provider;
                }
            }
        }
    }
}
