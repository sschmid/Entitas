using Entitas;

public class HasDependencySystemSpy :
    // Can implement any ISystem interface
    IHasSystemDependency<InitializeSystemSpy>,
    IHasSystemDependency<TearDownSystemSpy> {

    public int teardownSystemDeps { private set; get; }
    public int initialiseSystemDeps { private set; get; }

    public void SetSystem(InitializeSystemSpy dependency)
    {
        initialiseSystemDeps ++;
    }
    
    public void SetSystem(TearDownSystemSpy dependency)
    {
        teardownSystemDeps ++;
    }
}
