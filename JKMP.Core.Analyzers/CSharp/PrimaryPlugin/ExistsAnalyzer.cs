using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JKMP.Core.Analyzers.CSharp.PrimaryPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExistsAnalyzer : JkmpDiagnosticAnalyzer
{
    protected override void Analyze(AnalysisContext context)
    {
        context.RegisterCompilationAction(compilationContext =>
        {
            var basePluginType = GetCorePluginType(compilationContext.Compilation);

            if (basePluginType == null)
                return;

            var assembly = compilationContext.Compilation.Assembly;
            string? primaryPluginName = GetPrimaryPluginName(assembly);

            if (primaryPluginName == null)
                return;

            INamedTypeSymbol? pluginType = compilationContext.Compilation.GetTypeByMetadataName($"{assembly.Name}.{primaryPluginName}");

            if (pluginType == null)
            {
                compilationContext.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.JKMP1003_PrimaryPluginNotFound,
                    Location.None,
                    primaryPluginName,
                    assembly.Name
                ));
            }
        });
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Descriptors.JKMP1003_PrimaryPluginNotFound
    );
}
