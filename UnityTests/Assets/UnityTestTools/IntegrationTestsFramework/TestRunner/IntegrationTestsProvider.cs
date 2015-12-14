using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest.IntegrationTestRunner
{
    class IntegrationTestsProvider
    {
        internal Dictionary<ITestComponent, HashSet<ITestComponent>> testCollection = new Dictionary<ITestComponent, HashSet<ITestComponent>>();
        internal ITestComponent currentTestGroup;
        internal IEnumerable<ITestComponent> testToRun;

        public IntegrationTestsProvider(IEnumerable<ITestComponent> tests)
        {
            testToRun = tests;
            foreach (var test in tests.OrderBy(component => component))
            {
                if (test.IsTestGroup())
                {
                    throw new Exception(test.Name + " is test a group");
                }
                AddTestToList(test);
            }
            if (currentTestGroup == null)
            {
                currentTestGroup = FindInnerTestGroup(TestComponent.NullTestComponent);
            }
        }

        private void AddTestToList(ITestComponent test)
        {
            var group = test.GetTestGroup();
            if (!testCollection.ContainsKey(group))
                testCollection.Add(group, new HashSet<ITestComponent>());
            testCollection[group].Add(test);
            if (group == TestComponent.NullTestComponent) return;
            AddTestToList(group);
        }

        public ITestComponent GetNextTest()
        {
            var test = testCollection[currentTestGroup].First();
            testCollection[currentTestGroup].Remove(test);
            test.EnableTest(true);
            return test;
        }

        public void FinishTest(ITestComponent test)
        {
            try
            {
                test.EnableTest(false);
                currentTestGroup = FindNextTestGroup(currentTestGroup);
            }
            catch (MissingReferenceException e)
            {
                Debug.LogException(e);
            }
        }

        private ITestComponent FindNextTestGroup(ITestComponent testGroup)
        {
            if (testGroup == null) 
                throw new Exception ("No test left");

            if (testCollection[testGroup].Any())
            {
                testGroup.EnableTest(true);
                return FindInnerTestGroup(testGroup);
            }
            testCollection.Remove(testGroup);
            testGroup.EnableTest(false);

            var parentTestGroup = testGroup.GetTestGroup();
            if (parentTestGroup == null) return null;

            testCollection[parentTestGroup].Remove(testGroup);
            return FindNextTestGroup(parentTestGroup);
        }

        private ITestComponent FindInnerTestGroup(ITestComponent group)
        {
            var innerGroups = testCollection[group];
            foreach (var innerGroup in innerGroups)
            {
                if (!innerGroup.IsTestGroup()) continue;
                innerGroup.EnableTest(true);
                return FindInnerTestGroup(innerGroup);
            }
            return group;
        }

        public bool AnyTestsLeft()
        {
            return testCollection.Count != 0;
        }

        public List<ITestComponent> GetRemainingTests()
        {
            var remainingTests = new List<ITestComponent>();
            foreach (var test in testCollection)
            {
                remainingTests.AddRange(test.Value);
            }
            return remainingTests;
        }
    }
}
