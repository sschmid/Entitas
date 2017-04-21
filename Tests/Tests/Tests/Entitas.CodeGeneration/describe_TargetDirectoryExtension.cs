using NSpec;
using Entitas.CodeGeneration.Plugins;

class describe_TargetDirectoryExtension : nspec {

    void safe_dir() {
        it["appends '/Generated'"] = () => "Assets".ToSafeDirectory().should_be("Assets/Generated");
        it["appends 'Generated'"] = () => "Assets/".ToSafeDirectory().should_be("Assets/Generated");
        it["doesn't append"] = () => "Assets/Generated".ToSafeDirectory().should_be("Assets/Generated");
        it["removes trailing '/'"] = () => "Assets/Generated/".ToSafeDirectory().should_be("Assets/Generated");
        it["appends 'Generated'"] = () => "/".ToSafeDirectory().should_be("/Generated");
        it["appends 'Generated'"] = () => "".ToSafeDirectory().should_be("Generated");
        it["appends 'Generated'"] = () => ".".ToSafeDirectory().should_be("Generated");
    }
}
