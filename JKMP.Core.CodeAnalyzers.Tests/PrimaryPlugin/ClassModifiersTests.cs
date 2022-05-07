using System.Threading.Tasks;
using JKMP.Core.CodeAnalyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.CodeAnalyzers.Tests.PrimaryPlugin;

[TestClass]
public class ClassModifiersTests
{
    [TestMethod]
    public async Task PrimaryPluginIsNotAbstract()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class TestPlugin : JKMP.Core.Plugins.Plugin
{
}
";

        await CSharpVerifier<ClassModifiersAnalyzer>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task PrimaryPluginIsAbstract()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public abstract class TestPlugin : JKMP.Core.Plugins.Plugin
{
}

";

        await CSharpVerifier<ClassModifiersAnalyzer>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1002_PrimaryPluginMustBeNonAbstract)
                .WithArguments("TestPlugin")
                .WithSpan(4, 23, 4, 33)
        );
    }

    [TestMethod]
    public async Task NonPrimaryPluginIsAbstract()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public abstract class AbstractPlugin : JKMP.Core.Plugins.Plugin
{
}
";

        await CSharpVerifier<ClassModifiersAnalyzer>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task NonPrimaryPluginIsNotAbstract()
    {
        string code = @"
namespace JKMP.Plugin.Test;

public class OtherPlugin : JKMP.Core.Plugins.Plugin
{
}
";
        await CSharpVerifier<ClassModifiersAnalyzer>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1005_NonAbstractPublicPluginFound)
                .WithArguments("OtherPlugin")
                .WithSpan(4, 14, 4, 25)
        );
    }
}
