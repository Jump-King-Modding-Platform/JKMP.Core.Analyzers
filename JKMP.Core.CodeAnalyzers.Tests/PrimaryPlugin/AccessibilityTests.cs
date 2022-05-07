using System.Threading.Tasks;
using JKMP.Core.CodeAnalyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.CodeAnalyzers.Tests.PrimaryPlugin;

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
