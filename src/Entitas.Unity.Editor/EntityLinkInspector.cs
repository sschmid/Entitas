using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    [CustomEditor(typeof(EntityLink))]
    public class EntityLinkInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var link = (EntityLink)target;

            if (link.Entity != null)
                if (GUILayout.Button("Unlink"))
                    link.Unlink();

            if (link.Entity != null)
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(link.Entity.ToString());

                if (GUILayout.Button("Show entity"))
                    Selection.activeGameObject = FindObjectsOfType<EntityBehaviour>()
                        .Single(entity => entity.Entity == link.Entity).gameObject;

                EditorGUILayout.Space();

                EntityDrawer.DrawEntity(link.Entity);
            }
            else
            {
                EditorGUILayout.LabelField("Not linked to an entity");
            }
        }
    }
}
