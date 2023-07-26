using System.Linq;
using UnityEditor;

namespace Entitas.Unity.Editor
{
    [CustomEditor(typeof(EntityBehaviour)), CanEditMultipleObjects]
    public class EntityInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (targets.Length == 1)
            {
                EntityDrawer.DrawEntity(((EntityBehaviour)target).Entity);
            }
            else
            {
                var entities = targets
                    .Select(t => ((EntityBehaviour)t).Entity)
                    .ToArray();

                EntityDrawer.DrawMultipleEntities(entities);
            }

            if (target != null)
                EditorUtility.SetDirty(target);
        }
    }
}
