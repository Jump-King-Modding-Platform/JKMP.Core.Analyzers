using System.Threading.Tasks;
using JKMP.Core.CodeAnalyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.CodeAnalyzers.Tests.PrimaryPlugin;

[TestClass]
public class ExistsTests
{
    [TestMethod]
    public async Task PrimaryPluginFound()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class TestPlugin : JKMP.Core.Plugins.Plugin
{

}

";

        await CSharpVerifier<ExistsAnalyzer>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task PrimaryPluginNotFound_WrongNamespace()
    {
        string code = @"
public class TestPlugin : JKMP.Core.Plugins.Plugin
{
}
";

        await CSharpVerifier<ExistsAnalyzer>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1003_PrimaryPluginNotFound)
                .WithArguments(
                    "TestPlugin", // Plugin name
                    "JKMP.Plugin.Test" // The namespace it's supposed to be in
                )
                .WithNoLocation()
        );
    }

    [TestMethod]
    public async Task PrimaryPluginNotFound_WrongClassName()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class MyTestPlugin : JKMP.Core.Plugins.Plugin
{
}
";

        await CSharpVerifier<ExistsAnalyzer>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1003_PrimaryPluginNotFound)
                .WithArguments(
                    "TestPlugin", // Plugin name
                    "JKMP.Plugin.Test" // The namespace it's supposed to be in
                )
                .WithNoLocation()
        );
    }
}
