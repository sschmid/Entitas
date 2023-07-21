using Entitas;

public class CleanupSystemSpy : ICleanupSystem
{
    public int DidCleanup => _didCleanup;

    int _didCleanup;

    public void Cleanup() => _didCleanup += 1;
}
