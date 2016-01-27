using Entitas.Unity;
using UnityEditor;

public class TestPreferencesDrawer : IEntitasPreferencesDrawer {

    object _someObject;

    public void Initialize(EntitasPreferencesConfig config) {
        _someObject = new object();
    }

    public void Draw(EntitasPreferencesConfig config) {
        EditorGUILayout.LabelField("Test " + _someObject.GetHashCode());
    }
}
