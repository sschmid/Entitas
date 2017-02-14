using UnityEditor;
using Entitas.Unity.CodeGenerator;

public static class Commands {

    public static void GenerateCSharpProject() {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        UnityCodeGenerator.Generate();
    }
}
