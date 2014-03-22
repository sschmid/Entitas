using System;
using System.Threading;

namespace PerformanceTests {
    class MainClass {
        public static void Main(string[] args) {
            Console.WriteLine("Running performance tests...");
            Thread.Sleep(500);
//            run<EntityRepositoryCreateEntity>();
//            run<EntityRepositoryDestroyAllEntites>();
//            run<EntityRepositoryHasComponent>();
//            run<EntityRepositoryGetEntities>();
//            run<EntityGetComponents>();
//            run<EntityHasComponent>();
//            run<EntityMatcherGetHashCode>();
//            run<EntityMatcherEquals>();
//            run<EntityRemoveAddComponent>();
//            run<EntityReplaceComponet>();
//            run<EntityRepositoryOnEntityReplaced>();
            run<EntityGetComponent>();
//            run<ListAdd>();
//            run<OrderedSetAdd>();
//            run<LinkedListAdd>();
//            run<OrderedDictionaryAdd>();
//            run<ListRemove>();
//            run<LinkedListRemove>();
//            run<OrderedSetRemove>();
//            run<OrderedDictionaryRemove>();
//            run<ListGetItem>();
//            run<DictionaryGetItem>();
//            run<OrderedDictionaryGetItemByKey>();
//            run<OrderedDictionaryGetItemByIndex>();
//            run<ArrayGetItem>();
            Console.Read();
        }
        // Add: List (103), LinkedList (107), OrderedDictionary (122), OrderedSet (141)
        // Remove: LinkedList (3), OrderedDictionary (7), OrderdSet (12)
        // GetItem: Array (3.2), List (3.7), OrderedDictionaryByIndex (6.4), OrderedDictionaryByKey (6.5), Dictionary (7.5)
        static void run<T>() where T : IPerformanceTest, new() {
            Console.Write(typeof(T) + ": ");
            PerformanceTestRunner.Run(new T());
            var ticks = PerformanceTestRunner.Run(new T());
            var ms = ticks * 0.0001;
            Console.WriteLine(ms + " ms");
        }
    }
}
