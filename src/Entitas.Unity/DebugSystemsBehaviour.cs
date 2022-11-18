using UnityEngine;

namespace Entitas.Unity
{
    public class DebugSystemsBehaviour : MonoBehaviour
    {
        public DebugSystems systems => _systems;

        DebugSystems _systems;

        public void Init(DebugSystems systems)
        {
            _systems = systems;
        }
    }
}
