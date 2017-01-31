using System;

namespace Entitas.CodeGenerator {

    public interface IComponentDataProvider {

        void Provide(Type type, ComponentData data);
    }
}
