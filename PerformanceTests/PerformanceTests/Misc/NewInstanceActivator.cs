using System;

public class NewInstanceActivator : IPerformanceTest {
    const int n = 1000000;

    Type _type;

    public void Before() {
        _type = typeof(ComponentA);
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            createInstance();
        }
    }

    void createInstance() {
        Activator.CreateInstance(_type);
    }
}

