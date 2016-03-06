public class NewInstanceT : IPerformanceTest {
    const int n = 1000000;

    public void Before() {
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            createInstance<ComponentA>();
        }
    }

    void createInstance<T>() where T : new() {
        new T();
    }
}

