using System;

namespace Entitas
{
    /// Automatic Entity Reference Counting (AERC)
    /// is used internally to prevent pooling retained entities.
    /// If you use retain manually you also have to
    /// release it manually at some point.
    /// UnsafeAERC doesn't check if the entity has already been
    /// retained or released. It's faster than SafeAERC, but you lose the information
    /// about the owners.
    public sealed class UnsafeAERC : IAERC
    {
        public static readonly Func<Entity, IAERC> Delegate = entity => new UnsafeAERC();

        public int RetainCount => _retainCount;

        int _retainCount;

        public void Retain(object owner) => _retainCount += 1;

        public void Release(object owner) => _retainCount -= 1;
    }
}
