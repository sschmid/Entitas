using Entitas.Unity.Editor;
using UnityEditor;
using Entitas;
using System;

public static class TypeTest {

    [MenuItem("TEST/TYPE TEST")]
    public static void Start() {
        var types = TypeUtils.GetNonAbstractTypes<IEntitasPreferencesDrawer>();

        foreach(var type in types) {
            UnityEngine.Debug.Log("type: " + (type));
        }
    }
}
