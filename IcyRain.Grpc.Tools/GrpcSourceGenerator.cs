using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;

namespace IcyRain.Grpc.Tools;

[Generator]
public class GrpcSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        IncrementalValuesProvider<AdditionalText> additionalTexts = initContext.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".irs", StringComparison.OrdinalIgnoreCase));

        IncrementalValuesProvider<(string filePath, string content)> transformed = additionalTexts
            .Select(static (text, _) => (text.Path, text.GetText()?.ToString()));

        IncrementalValueProvider<ImmutableArray<(string filePath, string content)>> collected = transformed.Collect();

        initContext.RegisterSourceOutput(collected, static (sourceProductionContext, additionalTexts) =>
        {
            foreach (var (filePath, content) in additionalTexts)
            {
                string sourceFileName = Path.GetFileNameWithoutExtension(filePath) + ".g.cs";
                var service = new Service(content);
                string sourceContent = ServiceBuilder.Build(service);
                sourceProductionContext.AddSource(sourceFileName, sourceContent);
            }
        });
    }

}
