using System.Threading.Tasks;
using JKMP.Core.Analyzers.CSharp.PrimaryPlugin;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JKMP.Core.Analyzers.Tests.PrimaryPlugin;

[TestClass]
public class ConstructorTests
{
    /// <summary>
    /// {0} = class name
    /// {1} = constructor parameters
    /// </summary>
    private const string CodeTemplate = @"
using JKMP.Core.Plugins;

public class {0} : Plugin
{{
    public {0}({1})
    {{
    }}
}}
";

    [TestMethod]
    public async Task AllowEmptyConstructor()
    {
        string code = string.Format(CodeTemplate, "TestPlugin", "");
        await CSharpVerifier<ConstructorAnalyzers>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task AllowAtleastOnePublicEmptyConstructor()
    {
        string code2 = @"
using JKMP.Core.Plugins;

public class {0} : Plugin
{{
    public {0}()
    {{
    }}

    public {0}(string param)
    {{
    }}
}}
";

        await CSharpVerifier<ConstructorAnalyzers>.VerifyAnalyzer(string.Format(code2, "TestPlugin"));
    }

    [TestMethod]
    public async Task AllowNoConstructors()
    {
        string code = @"
using JKMP.Core.Plugins;

public class TestPlugin : Plugin
{
}
";

        await CSharpVerifier<ConstructorAnalyzers>.VerifyAnalyzer(code);
    }

    [TestMethod]
    public async Task PreventNonEmptyPublicConstructors()
    {
        string code = string.Format(CodeTemplate, "TestPlugin", "string invalidParam");
        await CSharpVerifier<ConstructorAnalyzers>.VerifyAnalyzer(
            code,
            new DiagnosticResult(Descriptors.JKMP1000_PrimaryPluginDoesNotHaveParameterlessPublicConstructor)
                .WithArguments("TestPlugin")
                .WithSpan(4, 14, 4, 24)
        );
    }
}
