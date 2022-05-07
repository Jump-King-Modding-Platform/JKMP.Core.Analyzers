using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace JKMP.Core.Analyzers;

public static class Descriptors
{
    static readonly ConcurrentDictionary<Category, string> Categories = new();
    static DiagnosticDescriptor Descriptor(string id, Category category, string title, string? messageFormat, DiagnosticSeverity defaultSeverity)
    {
        return new DiagnosticDescriptor(
            id,
            title,
            messageFormat ?? title,
            Categories.GetOrAdd(category, cat => cat.ToString()),
            defaultSeverity,
            isEnabledByDefault: true,
            helpLinkUri: $"https://docs.jkmp.dev/docs/analyzer-errors/{id}"
        );
    }

    public static DiagnosticDescriptor JKMP1000_PrimaryPluginDoesNotHaveParameterlessPublicConstructor { get; } = Descriptor(
        "JKMP1000",
        Category.Usage,
        "Primary plugins need at least one public parameterless constructor.",
        "The primary plugin class {0} has no public constructor without parameters.",
        DiagnosticSeverity.Error
    );

    public static DiagnosticDescriptor JKMP1001_PrimaryPluginMustBePublic { get; } = Descriptor(
        "JKMP1001",
        Category.Usage,
        "The primary plugin must be public.",
        "The primary plugin class {0} is not public.",
        DiagnosticSeverity.Error
    );

    public static DiagnosticDescriptor JKMP1002_PrimaryPluginMustBeNonAbstract { get; } = Descriptor(
        "JKMP1002",
        Category.Usage,
        "The primary plugin can not be abstract.",
        "The primary plugin class {0} is abstract.",
        DiagnosticSeverity.Error
    );

    public static DiagnosticDescriptor JKMP1003_PrimaryPluginNotFound { get; } = Descriptor(
        "JKMP1003",
        Category.Usage,
        "The primary plugin was not found.",
        "The primary plugin was not found. Make sure there is a class called {0} in the {1} namespace.",
        DiagnosticSeverity.Error
    );

    public static DiagnosticDescriptor JKMP1004_PrimaryPluginMustDeriveFromPlugin { get; } = Descriptor(
        "JKMP1004",
        Category.Usage,
        "The primary plugin must derive from JKMP.Core.Plugins.Plugin.",
        "The primary plugin class {0} does not derive from {1}.",
        DiagnosticSeverity.Error
    );

    public static DiagnosticDescriptor JKMP1005_NonAbstractPublicPluginFound { get; } = Descriptor(
        "JKMP1005",
        Category.Usage,
        "Non-abstract public plugin found that doesn't match the primary plugin name. This plugin won't be loaded by Core automatically.",
        "The plugin class {0} is not abstract and is not the primary plugin. It will not be loaded by Core automatically.",
        DiagnosticSeverity.Warning
    );

    private enum Category
    {
        Usage,
    }
}
