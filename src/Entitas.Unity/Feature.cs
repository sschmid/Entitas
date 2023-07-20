namespace Entitas.Unity
{
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
    public class Feature : Entitas.VisualDebugging.Unity.DebugSystems
    {
        public Feature(string name) : base(name) { }

        public Feature() : base(true)
        {
            var typeName = DesperateDevs.Extensions.TypeExtension.ToCompilableString(GetType());
            var shortType = DesperateDevs.Extensions.TypeExtension.TypeName(typeName);
            var readableType = DesperateDevs.Extensions.StringExtension.ToSpacedCamelCase(shortType);
            initialize(readableType);
        }
    }
#elif (!ENTITAS_DISABLE_DEEP_PROFILING && DEVELOPMENT_BUILD)
    public class Feature : Entitas.Systems
    {
        public Feature(string name) : this() { }

        public Feature() { }

        public override void Initialize()
        {
            foreach (var system in _initializeSystems)
            {
                UnityEngine.Profiling.Profiler.BeginSample(system.GetType().FullName);
                system.Initialize();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public override void Execute()
        {
            foreach (var system in _executeSystems)
            {
                UnityEngine.Profiling.Profiler.BeginSample(system.GetType().FullName);
                system.Execute();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public override void Cleanup()
        {
            foreach (var system in _cleanupSystems)
            {
                UnityEngine.Profiling.Profiler.BeginSample(system.GetType().FullName);
                system.Cleanup();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        public override void TearDown()
        {
            foreach (var system in _tearDownSystems)
            {
                UnityEngine.Profiling.Profiler.BeginSample(system.GetType().FullName);
                system.TearDown();
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
    }
#else
    public class Feature : Entitas.Systems
    {
        public Feature(string name) { }

        public Feature() { }
    }
#endif
}
