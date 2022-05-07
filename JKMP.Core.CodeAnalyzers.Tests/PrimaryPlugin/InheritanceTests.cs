using System.Threading.Tasks;
using JKMP.Core.CodeAnalyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.CodeAnalyzers.Tests.PrimaryPlugin;

[TestClass]
public class InheritanceTests
{
    [TestMethod]
    public async Task PrimaryPluginInheritsFromBasePlugin()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class TestPlugin : JKMP.Core.Plugins.Plugin
{
}
";

        await CSharpVerifier<InheritanceAnalyzer>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task PrimaryPluginDoesNotInheritFromBasePlugin()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class TestPlugin
{
}
";

        await CSharpVerifier<InheritanceAnalyzer>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1004_PrimaryPluginMustDeriveFromPlugin)
                .WithArguments("TestPlugin", "JKMP.Core.Plugins.Plugin")
                .WithSpan(4, 14, 4, 24)
        );
    }
}
