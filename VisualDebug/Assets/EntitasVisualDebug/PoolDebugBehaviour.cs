using UnityEngine;
using Entitas;

namespace Entitas.Debug {
    public class PoolDebugBehaviour : MonoBehaviour {
        public Pool pool { get { return _pool; } }

        Pool _pool;

        public void Init(Pool pool) {
            _pool = pool;
        }
    }
}