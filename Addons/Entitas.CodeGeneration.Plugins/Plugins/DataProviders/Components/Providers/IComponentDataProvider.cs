using System;

namespace Entitas.CodeGeneration.Plugins {

    public interface IComponentDataProvider {

        void Provide(Type type, ComponentData data);
    }
}
