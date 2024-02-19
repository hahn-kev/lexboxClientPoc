using System.Runtime.CompilerServices;
using Argon;

namespace Tests;

public class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        var model = new DataModelTestBase().DbContext.Model;
        VerifyEntityFramework.Initialize(model);
        VerifierSettings.AddExtraSettings(s => s.TypeNameHandling = TypeNameHandling.Objects);
        VerifyDiffPlex.Initialize();
    }
}