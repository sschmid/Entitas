using System;
using System.Linq;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {

    [CustomEditor(typeof(EntityBehaviour)), CanEditMultipleObjects]
    public class EntityInspector : Editor {

        void Awake() {
            EntityDrawer.Initialize();
        }

        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();
            {
                if (targets.Length == 1) {
                    var entityBehaviour = (EntityBehaviour)target;
                    EntityDrawer.DrawEntity(entityBehaviour.pool, entityBehaviour.entity);
                } else {
                    var entityBehaviour = (EntityBehaviour)target;
                    var entities = targets
                        .Select(t => ((EntityBehaviour)t).entity)
                        .ToArray();

                    EntityDrawer.DrawMultipleEntities(entityBehaviour.pool, entities);
                }
            }
            var changed = EditorGUI.EndChangeCheck();
            if (changed && target != null) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
