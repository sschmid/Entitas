using UnityEditor;
using Entitas.CodeGeneration.Unity.Editor;

public static class Commands {

    public static void GenerateCSharpProject() {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        UnityCodeGenerator.Generate();
    }
}
