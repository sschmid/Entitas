using System.Linq;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(EntityBehaviour)), CanEditMultipleObjects]
    public class EntityInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            if (targets.Length == 1) {
                var entityBehaviour = (EntityBehaviour)target;
                EntityDrawer.DrawEntity(entityBehaviour.entity);
            } else {
                var entities = targets
                        .Select(t => ((EntityBehaviour)t).entity)
                        .ToArray();

                EntityDrawer.DrawMultipleEntities(entities);
            }

            if (target != null) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
