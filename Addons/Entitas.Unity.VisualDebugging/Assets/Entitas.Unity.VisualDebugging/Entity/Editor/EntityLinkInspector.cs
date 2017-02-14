using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    [CustomEditor(typeof(EntityLink))]
    public class EntityLinkInspector : Editor {

        void Awake() {
            EntityDrawer.Initialize();
        }

        public override void OnInspectorGUI() {
            var link = (EntityLink)target;

            if(link.entity != null) {
                if(GUILayout.Button("Unlink")) {
                    link.Unlink();
                }
            }

            if(link.entity != null) {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(link.entity.ToString());

                if(GUILayout.Button("Show entity")) {
                    Selection.activeGameObject = FindObjectsOfType<EntityBehaviour>()
                        .Single(e => e.entity == link.entity).gameObject;
                }

                EditorGUILayout.Space();

                EntityDrawer.DrawEntity(link.context, link.entity);
            } else {
                EditorGUILayout.LabelField("Not linked to an entity");
            }
        }
    }
}
