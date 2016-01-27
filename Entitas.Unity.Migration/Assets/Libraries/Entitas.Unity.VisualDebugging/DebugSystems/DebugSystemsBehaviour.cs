using Entitas.Unity.VisualDebugging;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class DebugSystemsBehaviour : MonoBehaviour {
        public DebugSystems systems { get { return _systems; } }

        DebugSystems _systems;

        public void Init(DebugSystems systems) {
            _systems = systems;
        }
    }
}