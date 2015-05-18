using System;
using System.Collections.Generic;

namespace Entitas {
    public class Systems {

        readonly List<IStartSystem> _startSystems;
        readonly List<IExecuteSystem> _executeSystems;

        public Systems() {
            _startSystems = new List<IStartSystem>();
            _executeSystems = new List<IExecuteSystem>();
        }

        public void Add<T>() {
            Add(typeof(T));
        }

        public void Add(Type systemType) {
            Add((ISystem)Activator.CreateInstance(systemType));
        }

        public void Add(ISystem system) {
            var reactiveSystem = system as ReactiveSystem;
            var startSystem = reactiveSystem != null
                ? reactiveSystem.subsystem as IStartSystem
                : system as IStartSystem;

            if (startSystem != null) {
                _startSystems.Add(startSystem);
            }

            var executeSystem = system as IExecuteSystem;
            if (executeSystem != null) {
                _executeSystems.Add(executeSystem);
            }
        }

        public void Start() {
            foreach (var system in _startSystems) {
                system.Start();
            }
        }

        public void Execute() {
            foreach (var system in _executeSystems) {
                system.Execute();
            }
        }
    }
}