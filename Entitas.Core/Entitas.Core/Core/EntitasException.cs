using System;

namespace Entitas.Core {

    /// Base exception used by Entitas.
    public class EntitasException : Exception {

        public EntitasException(string message, string hint)
            : base(hint != null ? (message + "\n" + hint) : message) {
        }
    }
}
