using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class PoolDebugBehaviour : MonoBehaviour {
        public DebugPool pool { get { return _pool; } }

        DebugPool _pool;

        public void Init(DebugPool pool) {
            _pool = pool;
        }
    }
}