using System;
using System.Threading;

namespace PerformanceTests {

    class MainClass {

        public static void Main(string[] args) {
            Console.WriteLine("Running performance tests...");
            Thread.Sleep(1500);

            run<ContextCreateEntity>();
            run<ContextDestroyEntity>();
            run<ContextDestroyAllEntities>();
            run<ContextGetGroup>();
            run<ContextGetEntities>();
            run<ContextHasEntity>();
            run<ContextOnEntityReplaced>();
            run<EmptyTest>();

            run<EntityAddComponent>();
            run<EntityGetComponent>();
            run<EntityGetComponents>();
            run<EntityHasComponent>();
            run<EntityRemoveAddComponent>();
            run<EntityReplaceComponent>();
            run<EmptyTest>();

            run<MatcherEquals>();
            run<MatcherGetHashCode>();

            run<ContextCreateBlueprint>();

            run<NewInstanceT>();
            run<NewInstanceActivator>();

            run<EntityIndexGetEntity>();
            run<EmptyTest>();

            run<ObjectGetProperty>();
            run<EmptyTest>();
            run<CollectorIterateCollectedEntities>();
            run<CollectorActivate>();

            run<Casting>();

            Console.WriteLine("\nPress any key...");
            Console.Read();
        }

        //Running performance tests...
        //ContextCreateEntity:                     30 ms
        //ContextDestroyEntity:                    29 ms
        //ContextDestroyAllEntities:               25 ms
        //ContextGetGroup:                          5 ms
        //ContextGetEntities:                       2 ms
        //ContextHasEntity:                        10 ms
        //ContextOnEntityReplaced:                  6 ms

        //EntityAddComponent:                     257 ms
        //EntityGetComponent:                      44 ms
        //EntityGetComponents:                      4 ms
        //EntityHasComponent:                       2 ms
        //EntityRemoveAddComponent:               289 ms
        //EntityReplaceComponent:                  20 ms

        //MatcherEquals:                          171 ms
        //MatcherGetHashCode:                      17 ms

        //ContextCreateBlueprint:                 256 ms

        //NewInstanceT:                           393 ms
        //NewInstanceActivator:                   542 ms
        //EntityIndexGetEntity:                    59 ms

        //IterateHashetToArray:                   456 ms
        //IterateHashSet:                         774 ms

        //ObjectGetProperty:                        6 ms

        //CollectorIterateCollectedEntities:      957 ms
        //CollectorActivate:                        1 ms
        //PropertiesCreate:                       251 ms

        static void run<T>() where T : IPerformanceTest, new() {
            Thread.Sleep(100);
            if(typeof(T) == typeof(EmptyTest)) {
                Console.WriteLine(string.Empty);
                return;
            }
            Console.Write((typeof(T) + ": ").PadRight(40));
            // For more reliable results, run before
            PerformanceTestRunner.Run(new T());
            var ms = PerformanceTestRunner.Run(new T());
            Console.WriteLine(ms + " ms");
        }
    }
}
