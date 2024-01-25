using lexboxClientContracts;
using TypeGen.Core.SpecGeneration;

namespace lexboxClientContracts;

public class TypeScriptGenerationSpec : GenerationSpec
{
    public override void OnBeforeGeneration(OnBeforeGenerationArgs args)
    {
        args.GeneratorOptions.BaseOutputDirectory = "../lexboxSvelte/src/lib/mini-lcm";
        args.GeneratorOptions.CustomTypeMappings.Add(typeof(WritingSystemId).FullName, "string");
        AddInterface<Entry>();
    }

    public override void OnBeforeBarrelGeneration(OnBeforeBarrelGenerationArgs args)
    {
        AddBarrel(".");
    }
}