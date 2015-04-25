using Entitas.Unity;
using UnityEditor;

public class TestPreferencesDrawer : IEntitasPreferencesDrawer {
    public void Draw(EntitasPreferencesConfig config) {
        EditorGUILayout.LabelField("Test");
    }
}
