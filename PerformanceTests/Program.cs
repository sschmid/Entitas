using System;
using System.Threading;

namespace PerformanceTests {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine("Running performance tests...");
            Thread.Sleep(500);
            run<EntityRepositoryCreateEntity>();
            run<EntityRepositoryDestroyAllEntites>();
            run<EntityRepositoryHasComponent>();
            run<EntityRepositoryGetEntities>();
            run<EntityGetComponents>();
            run<EntityHasComponent>();
            run<EntityMatcherGetHashCode>();
            run<EntityMatcherEquals>();
            run<EntityRemoveAddComponent>();
            run<EntityReplaceComponet>();
            Console.Read();
        }

        static void run<T>() where T : IPerformanceTest, new() {
            Console.Write(typeof(T) + ": ");
            PerformanceTestRunner.Run(new T());
            var ticks = PerformanceTestRunner.Run(new T());
            var ms = ticks * 0.0001;
            Console.WriteLine(ms + " ms");
        }
    }
}
