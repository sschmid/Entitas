//using NSpec;
//using Entitas.CodeGenerator;

//class describe_CodeGenerator : nspec {

//    void safe_dir() {
//        it["appends '/Generated/'"] = () => CodeGenerator.GetSafeDir("Assets").should_be("Assets/Generated/");
//        it["appends 'Generated/'"] = () => CodeGenerator.GetSafeDir("Assets/").should_be("Assets/Generated/");
//        it["appends '/'"] = () => CodeGenerator.GetSafeDir("Assets/Generated").should_be("Assets/Generated/");
//        it["doesn't append"] = () => CodeGenerator.GetSafeDir("Assets/Generated/").should_be("Assets/Generated/");
//        it["appends 'Generated/'"] = () => CodeGenerator.GetSafeDir("/").should_be("/Generated/");
//    }
//}
