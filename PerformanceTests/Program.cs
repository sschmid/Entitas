using System;
using System.Threading;

namespace PerformanceTests {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine("Running performance tests...");
            Thread.Sleep(1500);

            run<EntityRepositoryCreateEntity>();
            run<EntityRepositoryDestroyAllEntites>();
            run<EntityRepositoryGetCollection>();
            run<EntityRepositoryGetEntities>();
            run<EntityRepositoryHasEntity>();
            run<EntityRepositoryOnEntityReplaced>();
            run<EmptyTest>();

            run<EntityGetComponent>();
            run<EntityGetComponents>();
            run<EntityHasComponent>();
            run<EntityRemoveAddComponent>();
            run<EntityReplaceComponent>();
            run<EmptyTest>();

            run<EntityMatcherEquals>();
            run<EntityMatcherGetHashCode>();
            run<EmptyTest>();

//            run<ArrayGetItem>();
//            run<DictionaryGetItem>();
//            run<LinkedListAdd>();
//            run<LinkedListRemove>();
//            run<ListAdd>();
//            run<ListGetItem>();
//            run<ListRemove>();
//            run<OrderedDictionaryAdd>();
//            run<OrderedDictionaryGetItemByIndex>();
//            run<OrderedDictionaryGetItemByKey>();
//            run<OrderedDictionaryRemove>();
//            run<OrderedSetAdd>();
//            run<OrderedSetRemove>();

            Console.WriteLine("\nPress any key...");
            Console.Read();
        }

        //        Running performance tests...
        //        EntityRepositoryCreateEntity:           109 ms
        //        EntityRepositoryDestroyAllEntites:      49 ms
        //        EntityRepositoryGetEntities:            3 ms
        //        EntityRepositoryHasEntity:              8 ms
        //        EntityRepositoryOnEntityReplaced:       8 ms
        //
        //        EntityGetComponent:                     43 ms
        //        EntityGetComponents:                    3 ms
        //        EntityHasComponent:                     1 ms
        //        EntityRemoveAddComponent:               396 ms
        //        EntityReplaceComponent:                 22 ms
        //
        //        EntityMatcherEquals:                    242 ms
        //        EntityMatcherGetHashCode:               21 ms
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
        //        OrderedSetAdd:                          44 ms
        //        OrderedSetRemove:                       13 ms

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
