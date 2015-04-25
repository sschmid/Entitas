using Entitas.Unity;
using UnityEditor;

public class OtherTestPreferencesDrawer : IEntitasPreferencesDrawer {
    public void Draw(EntitasPreferencesConfig config) {
        EditorGUILayout.LabelField("Other Test");
    }
}
