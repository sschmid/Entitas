using UnityEditor;

public static class Commands {

    public static void GenerateCSharpProject() {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
}
