using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.Analyzers;

public abstract class JkmpDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        Analyze(context);
        context.RegisterCompilationStartAction(AnalyzeCompilation);
    }

    protected virtual void Analyze(AnalysisContext context)
    {
    }

    #pragma warning disable RS1012
    protected virtual void AnalyzeCompilation(CompilationStartAnalysisContext compilationContext)
    {
    }
    #pragma warning restore RS1012

    protected string? GetPrimaryPluginName(IAssemblySymbol assemblySymbol)
    {
        var assemblyName = assemblySymbol.Name;

        if (!assemblyName.StartsWith("JKMP.Plugin."))
            return null;

        var pluginName = assemblyName.Substring("JKMP.Plugin.".Length);
        return pluginName + "Plugin";
    }

    protected bool TypeMatchesPrimaryPluginName(INamedTypeSymbol type)
    {
        string? primaryPluginName = GetPrimaryPluginName(type.ContainingAssembly);

        if (primaryPluginName == null)
            return false;

        return type.Name == primaryPluginName;
    }

    protected static INamedTypeSymbol? GetCorePluginType(Compilation compilation)
    {
        var pluginType = compilation.GetTypeByMetadataName(Constants.PluginClassName);
        return pluginType;
    }
}
