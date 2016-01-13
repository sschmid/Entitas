using UnityEditor;

public static class Commands {
    public static void GenerateProjectFiles() {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
}
