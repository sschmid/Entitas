using System.Linq;
using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(EntityLink))]
    public class EntityLinkInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var link = (EntityLink)target;

            if (link.entity != null) {
                if (GUILayout.Button("Unlink")) {
                    link.Unlink();
                }
            }

            if (link.entity != null) {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(link.entity.ToString());

                if (GUILayout.Button("Show entity")) {
                    Selection.activeGameObject = FindObjectsOfType<EntityBehaviour>()
                        .Single(e => e.entity == link.entity).gameObject;
                }

                EditorGUILayout.Space();

                EntityDrawer.DrawEntity(link.entity);
            } else {
                EditorGUILayout.LabelField("Not linked to an entity");
            }
        }
    }
}
