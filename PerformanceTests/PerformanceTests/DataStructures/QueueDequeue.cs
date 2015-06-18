using System.Collections.Generic;

public class QueueDequeue : IPerformanceTest {
    const int n = 1000000;
    const int elements = 60;
    Queue<float> _q;

    public void Before() {
        _q = new Queue<float>(elements);
        for (int i = 0; i < elements; i++) {
            _q.Enqueue(1f);
        }
    }

    public void Run() {
        for (int i = 0; i < n; i++) {
            _q.Dequeue();
            _q.Enqueue(1f);
        }
    }
}


