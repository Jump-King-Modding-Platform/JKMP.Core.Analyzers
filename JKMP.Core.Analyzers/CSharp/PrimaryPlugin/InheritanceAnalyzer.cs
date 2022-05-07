using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.Analyzers.CSharp.PrimaryPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InheritanceAnalyzer : PrimaryPluginDiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Descriptors.JKMP1004_PrimaryPluginMustDeriveFromPlugin
    );

    protected override void AnalyzePrimaryPluginSymbol(INamedTypeSymbol type, SymbolAnalysisContext symbolContext)
    {
        var pluginType = GetCorePluginType(symbolContext.Compilation);

        if (pluginType == null)
            return;

        if (!SymbolEqualityComparer.IncludeNullability.Equals(pluginType, type.BaseType))
        {
            symbolContext.ReportDiagnostic(Diagnostic.Create(
                Descriptors.JKMP1004_PrimaryPluginMustDeriveFromPlugin,
                symbolContext.Symbol.Locations.First(),
                type.Name,
                $"{pluginType.ContainingNamespace}.{pluginType.Name}"
            ));
        }
    }
}
