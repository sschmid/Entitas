using NSpec;
using Entitas.CodeGenerator;

class describe_Writer : nspec {
    void safe_dir() {
        it["appends '/Generated/'"] = () => WriteToDirectoryProcessor.GetSafeDir("Assets").should_be("Assets/Generated/");
        it["appends 'Generated/'"] = () => WriteToDirectoryProcessor.GetSafeDir("Assets/").should_be("Assets/Generated/");
        it["appends '/'"] = () => WriteToDirectoryProcessor.GetSafeDir("Assets/Generated").should_be("Assets/Generated/");
        it["doesn't append"] = () => WriteToDirectoryProcessor.GetSafeDir("Assets/Generated/").should_be("Assets/Generated/");
        it["appends 'Generated/'"] = () => WriteToDirectoryProcessor.GetSafeDir("/").should_be("/Generated/");
    }
}

