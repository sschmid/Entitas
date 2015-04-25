using Entitas.Unity;
using UnityEditor;

public class OtherTestPreferencesDrawer : IEntitasPreferencesDrawer {
    public void Draw(string configPath) {
        EditorGUILayout.LabelField("Other Test");
    }
}
