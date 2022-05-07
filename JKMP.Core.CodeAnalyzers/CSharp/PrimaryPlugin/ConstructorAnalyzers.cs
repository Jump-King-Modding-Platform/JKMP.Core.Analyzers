using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.CodeAnalyzers.CSharp.PrimaryPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConstructorAnalyzers : PrimaryPluginDiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = new[]
    {
        Descriptors.JKMP1000_PrimaryPluginDoesNotHaveParameterlessPublicConstructor
    }.ToImmutableArray();

    protected override void AnalyzePrimaryPluginSymbol(INamedTypeSymbol type, SymbolAnalysisContext symbolContext)
    {
        if (type.IsAbstract)
            return;

        if (!type.Constructors.Any())
            return;

        if (type.Constructors.Where(constructor => constructor.DeclaredAccessibility == Accessibility.Public).All(constructor => constructor.Parameters.Length > 0))
        {
            symbolContext.ReportDiagnostic(Diagnostic.Create(
                Descriptors.JKMP1000_PrimaryPluginDoesNotHaveParameterlessPublicConstructor,
                symbolContext.Symbol.Locations.First(),
                symbolContext.Symbol.Name
            ));
        }
    }
}
