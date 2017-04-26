using System;

namespace Entitas.CodeGeneration.Attributes {

    [Flags]
    public enum EntityAccessType {
        None        = 0,
        Read        = 1 << 1,
        Write       = 1 << 2,
        ReadWrite   = Read | Write
    }
}
