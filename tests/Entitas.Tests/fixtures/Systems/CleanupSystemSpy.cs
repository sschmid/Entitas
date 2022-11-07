using Entitas;

public class CleanupSystemSpy : ICleanupSystem {

    public int didCleanup { get { return _didCleanup; } }

    int _didCleanup;

    public void Cleanup() {
        _didCleanup += 1;
    }
}
