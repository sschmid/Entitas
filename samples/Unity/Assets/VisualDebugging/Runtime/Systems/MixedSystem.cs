using Entitas;

public class MixedSystem : IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem {
 
    public void Initialize() {
        //UnityEngine.Debug.Log("Initialize");
    }

    public void Execute() {
        //UnityEngine.Debug.Log("Execute");
    }

    public void Cleanup() {
        //UnityEngine.Debug.Log("Cleanup");
    }

    public void TearDown() {
        //UnityEngine.Debug.Log("TearDown");
    }
}
