using Entitas.Api;

public class CleanupSystem : ICleanupSystem {

    public void Cleanup() {
        UnityEngine.Debug.Log("Cleanup");
    }
}
