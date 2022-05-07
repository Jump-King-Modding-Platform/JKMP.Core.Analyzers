using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.Analyzers.CSharp.PrimaryPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ClassModifiersAnalyzer : PrimaryPluginDiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Descriptors.JKMP1002_PrimaryPluginMustBeNonAbstract,
        Descriptors.JKMP1005_NonAbstractPublicPluginFound
    );

    protected override void Analyze(AnalysisContext context)
    {
        base.Analyze(context);

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var pluginType = GetCorePluginType(compilationContext.Compilation);

            if (pluginType == null)
                return;

            compilationContext.RegisterSymbolAction(symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;

                if (!SymbolEqualityComparer.IncludeNullability.Equals(type.BaseType, pluginType))
                    return;

                if (TypeMatchesPrimaryPluginName(type))
                    return;

                if (type.DeclaredAccessibility != Accessibility.Public)
                    return;

                if (type.IsAbstract)
                    return;

                symbolContext.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.JKMP1005_NonAbstractPublicPluginFound,
                    symbolContext.Symbol.Locations.First(),
                    symbolContext.Symbol.Name
                ));
            }, SymbolKind.NamedType);
        });
    }

    protected override void AnalyzePrimaryPluginSymbol(INamedTypeSymbol type, SymbolAnalysisContext symbolContext)
    {
        // Report if plugin is abstract
        if (type.IsAbstract)
        {
            symbolContext.ReportDiagnostic(Diagnostic.Create(
                Descriptors.JKMP1002_PrimaryPluginMustBeNonAbstract,
                symbolContext.Symbol.Locations.First(),
                type.Name
            ));
        }
    }
}
