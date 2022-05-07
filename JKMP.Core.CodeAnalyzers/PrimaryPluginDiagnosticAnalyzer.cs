using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.CodeAnalyzers;

public abstract class PrimaryPluginDiagnosticAnalyzer : JkmpDiagnosticAnalyzer
{
    protected sealed override void AnalyzeCompilation(CompilationStartAnalysisContext compilationContext)
    {
        var pluginType = GetCorePluginType(compilationContext.Compilation);

        if (pluginType == null)
            return;

        compilationContext.RegisterSymbolAction(symbolContext =>
        {
            var type = (INamedTypeSymbol)symbolContext.Symbol;

            if (!TypeMatchesPrimaryPluginName(type))
                return;

            AnalyzePrimaryPluginSymbol(type, symbolContext);
        }, SymbolKind.NamedType);
    }

    protected abstract void AnalyzePrimaryPluginSymbol(INamedTypeSymbol type, SymbolAnalysisContext symbolContext);
}
