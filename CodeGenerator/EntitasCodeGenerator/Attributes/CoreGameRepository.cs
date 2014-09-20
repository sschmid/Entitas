using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class CoreGameRepository : EntityRepositoryAttribute {
        public CoreGameRepository() : base("CoreGameComponentIds") {
        }
    }
}