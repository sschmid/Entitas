using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityRepositoryAttribute : Attribute {
        public string tag;

        public EntityRepositoryAttribute(string tag) {
            this.tag = tag;
        }
    }
}