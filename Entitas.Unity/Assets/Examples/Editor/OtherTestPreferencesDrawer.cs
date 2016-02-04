using Entitas.Unity;
using UnityEditor;
using UnityEngine;

public class OtherTestPreferencesDrawer : IEntitasPreferencesDrawer {

    public int priority { get { return 2; } }

    object _someObject;

    public void Initialize(EntitasPreferencesConfig config) {
        _someObject = new object();
    }

    public void Draw(EntitasPreferencesConfig config) {
        EditorGUILayout.LabelField("Other Test " + _someObject.GetHashCode());
        GUILayout.Space(500);
    }
}
