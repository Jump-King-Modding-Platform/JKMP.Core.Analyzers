using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.Analyzers.CSharp.PrimaryPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AccessibilityAnalyzers : PrimaryPluginDiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Descriptors.JKMP1001_PrimaryPluginMustBePublic
    );
    protected override void AnalyzePrimaryPluginSymbol(INamedTypeSymbol type, SymbolAnalysisContext symbolContext)
    {
        if (type.DeclaredAccessibility != Accessibility.Public)
        {
            symbolContext.ReportDiagnostic(Diagnostic.Create(
                Descriptors.JKMP1001_PrimaryPluginMustBePublic,
                type.Locations[0],
                symbolContext.Symbol.Name
            ));
        }
    }
}
