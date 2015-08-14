using System;
using System.Threading;

namespace PerformanceTests {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine("Running performance tests...");
            Thread.Sleep(1500);

            run<PoolCreateEntity>();
            run<PoolDestroyAllEntities>();
            run<PoolGetGroup>();
            run<PoolGetEntities>();
            run<PoolHasEntity>();
            run<PoolOnEntityReplaced>();
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
            run<EmptyTest>();

            run<IterateHashetToArray>();
            run<IterateHashSet>();
            run<EmptyTest>();

            run<ObjectGetProperty>();
            run<EmptyTest>();
            run<ObserverIterateCollectedEntities>();
//            run<PropertiesCreate>();

            run<ArrayGetItem>();
            run<DictionaryGetItem>();
            run<QueueDequeue>();
            run<ListQueue>();
//            run<LinkedListAdd>();
//            run<LinkedListRemove>();
//            run<ListAdd>();
//            run<ListGetItem>();
//            run<ListRemove>();
//            run<OrderedDictionaryAdd>();
//            run<OrderedDictionaryGetItemByIndex>();
//            run<OrderedDictionaryGetItemByKey>();
//            run<OrderedDictionaryRemove>();

            Console.WriteLine("\nPress any key...");
            Console.Read();
        }

        //        Running performance tests...
        //        PoolCreateEntity:                       68 ms
        //        PoolDestroyAllEntities:                 49 ms
        //        PoolGetGroup:                           5 ms
        //        PoolGetEntities:                        23 ms
        //        PoolHasEntity:                          9 ms
        //        PoolOnEntityReplaced:                   6 ms
        //
        //        EntityAddComponent:                     272 ms
        //        EntityGetComponent:                     45 ms
        //        EntityGetComponents:                    4 ms
        //        EntityHasComponent:                     2 ms
        //        EntityRemoveAddComponent:               406 ms
        //        EntityReplaceComponent:                 20 ms
        //
        //        MatcherEquals:                          161 ms
        //        MatcherGetHashCode:                     26 ms
        //
        //        IterateHashetToArray:                   476 ms
        //        IterateHashSet:                         752 ms
        //
        //        ObjectGetProperty:                      6 ms
        //
        //        ObserverIterateCollectedEntities:       884 ms
        //        PropertiesCreate:                       251 ms
        //
        //        ArrayGetItem:                           2 ms
        //        DictionaryGetItem:                      7 ms
        //        LinkedListAdd:                          17 ms
        //        LinkedListRemove:                       4 ms
        //        ListAdd:                                14 ms
        //        ListGetItem:                            2 ms
        //        ListRemove:                             8647 ms
        //        OrderedDictionaryAdd:                   30 ms
        //        OrderedDictionaryGetItemByIndex:        4 ms
        //        OrderedDictionaryGetItemByKey:          4 ms
        //        OrderedDictionaryRemove:                7 ms

        static void run<T>() where T : IPerformanceTest, new() {
            Thread.Sleep(100);
            if (typeof(T) == typeof(EmptyTest)) {
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
