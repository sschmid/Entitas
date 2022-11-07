using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class MultiReactiveSystemTests
    {
        [Fact]
        public void ProcessesEntitiesFromDifferentContexts()
        {
            var contexts = new Contexts();
            var system = new MultiReactiveSystemSpy(contexts);
            system.executeAction = entities =>
            {
                foreach (var e in entities)
                    e.nameAge.age += 10;
            };

            var e1 = contexts.test.CreateEntity();
            e1.AddNameAge("Max", 42);

            var e2 = contexts.test2.CreateEntity();
            e2.AddNameAge("Jack", 24);

            system.Execute();

            system.entities.Length.Should().Be(2);
            system.entities.Should().Contain(e1);
            system.entities.Should().Contain(e2);

            e1.nameAge.age.Should().Be(52);
            e2.nameAge.age.Should().Be(34);
            system.didExecute.Should().Be(1);

            (e1.aerc as SafeAERC)?.owners.Should().HaveCount(2);
            (e2.aerc as SafeAERC)?.owners.Should().HaveCount(2);
        }

        [Fact]
        public void ExecutesOnce()
        {
            var contexts = new Contexts();
            var system = new MultiTriggeredMultiReactiveSystemSpy(contexts);
            var e = contexts.test.CreateEntity();
            e.AddNameAge("Max", 42);
            e.RemoveNameAge();
            system.Execute();
            system.didExecute.Should().Be(1);
            system.entities.Length.Should().Be(1);

            system.Execute();
            system.didExecute.Should().Be(1);
        }

        [Fact]
        public void CanToString()
        {
            new MultiReactiveSystemSpy(new Contexts()).ToString()
                .Should().Be("MultiReactiveSystem(MultiReactiveSystemSpy)");
        }
    }
}
