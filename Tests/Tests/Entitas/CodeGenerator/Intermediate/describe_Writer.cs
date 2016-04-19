using NSpec;
using Entitas.CodeGenerator;

class describe_Writer : nspec {
    void safe_dir() {
        it["appends '/Generated/'"] = () => Writer.GetSafeDir("Assets").should_be("Assets/Generated/");
        it["appends 'Generated/'"] = () => Writer.GetSafeDir("Assets/").should_be("Assets/Generated/");
        it["appends '/'"] = () => Writer.GetSafeDir("Assets/Generated").should_be("Assets/Generated/");
        it["doesn't append"] = () => Writer.GetSafeDir("Assets/Generated/").should_be("Assets/Generated/");
        it["appends 'Generated/'"] = () => Writer.GetSafeDir("/").should_be("/Generated/");
    }
}

