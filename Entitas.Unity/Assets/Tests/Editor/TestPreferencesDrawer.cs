using Entitas.Unity;
using UnityEditor;

public class TestPreferencesDrawer : IEntitasPreferencesDrawer {
    public void Draw(string configPath) {
        EditorGUILayout.LabelField("Test");
    }
}
