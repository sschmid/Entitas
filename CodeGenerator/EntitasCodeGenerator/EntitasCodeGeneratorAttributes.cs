using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class EntityRepositoryAttribute : Attribute {
        public string tag;

        public EntityRepositoryAttribute(string tag) {
            this.tag = tag;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class MetaGameRepository : EntityRepositoryAttribute {
        public MetaGameRepository() : base("MetaGameComponentIds") {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CoreGameRepository : EntityRepositoryAttribute {
        public CoreGameRepository() : base("CoreGameComponentIds") {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DontGenerate : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SingleEntity : Attribute {
    }
}