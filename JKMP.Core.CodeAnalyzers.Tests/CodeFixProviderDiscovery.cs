using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.VisualStudio.Composition;

namespace JKMP.Core.CodeAnalyzers.Tests;

public static class CodeFixProviderDiscovery
{
    private static readonly Lazy<IExportProviderFactory> ExportProviderFactory;

    static CodeFixProviderDiscovery()
    {
        ExportProviderFactory = new(
            () =>
            {
                var discovery = new AttributedPartDiscovery(Resolver.DefaultInstance, isNonPublicSupported: true);
                var parts = Task.Run(() => discovery.CreatePartsAsync(typeof(Descriptors).Assembly)).GetAwaiter().GetResult();
                var catalog = ComposableCatalog.Create(Resolver.DefaultInstance).AddParts(parts);

                var configuration = CompositionConfiguration.Create(catalog);
                var runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration);
                return runtimeComposition.CreateExportProviderFactory();
            }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public static IEnumerable<CodeFixProvider> GetCodeFixProviders(string language)
    {
        var exportProvider = ExportProviderFactory.Value.CreateExportProvider();
        var exports = exportProvider.GetExports<CodeFixProvider, LanguageMetaData>();

        return exports.Where(export => export.Metadata.Languages.Contains(language)).Select(export => export.Value);
    }

    private class LanguageMetaData
    {
        public ImmutableArray<string> Languages { get; }

        public LanguageMetaData(IDictionary<string, object> data)
        {
            if (!data.TryGetValue("Languages", out var languages))
            {
                languages = Array.Empty<string>();
            }

            Languages = ((string[])languages).ToImmutableArray();
        }
    }
}
