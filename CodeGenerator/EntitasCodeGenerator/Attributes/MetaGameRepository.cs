using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class MetaGameRepository : EntityRepositoryAttribute {
        public MetaGameRepository() : base("MetaGameComponentIds") {
        }
    }
}