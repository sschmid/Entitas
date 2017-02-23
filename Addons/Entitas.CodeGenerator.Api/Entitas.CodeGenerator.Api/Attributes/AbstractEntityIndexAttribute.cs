using System;

namespace Entitas.CodeGenerator.Api {

    public abstract class AbstractEntityIndexAttribute : Attribute {

        public readonly string type;
        public readonly bool isPrimary;

        protected AbstractEntityIndexAttribute(string type, bool isPrimary) {
            this.type = type;
            this.isPrimary = isPrimary;
        }
    }
}
