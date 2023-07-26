using UnityEngine;

namespace Entitas.Unity
{
    public class DebugSystemsBehaviour : MonoBehaviour
    {
        public DebugSystems Systems => _systems;

        DebugSystems _systems;

        public void Initialize(DebugSystems systems)
        {
            _systems = systems;
        }
    }
}
