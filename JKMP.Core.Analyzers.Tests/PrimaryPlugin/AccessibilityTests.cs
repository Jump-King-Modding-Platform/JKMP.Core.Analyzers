using System.Threading.Tasks;
using JKMP.Core.Analyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.Analyzers.Tests.PrimaryPlugin;

[TestClass]
public class AccessibilityTests
{
    [TestMethod]
    public async Task PublicPluginIsAccessible()
    {
        string code = @"
using JKMP.Core.Plugins;

public class TestPlugin : Plugin
{
}
";

        await CSharpVerifier<AccessibilityAnalyzers>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task InternalPluginIsInaccessible()
    {
        string code = @"
using JKMP.Core.Plugins;

internal class TestPlugin : Plugin
{
}
";

        await CSharpVerifier<AccessibilityAnalyzers>.VerifyAnalyzer(code,
            new DiagnosticResult(Descriptors.JKMP1001_PrimaryPluginMustBePublic)
                .WithArguments("TestPlugin")
                .WithSpan(4, 16, 4, 26)
        );
    }
}
