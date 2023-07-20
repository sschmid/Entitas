using System;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    [Collection(nameof(JobSystemTests))]
    public class JobSystemTests
    {
        readonly TestContext _context;

        public JobSystemTests()
        {
            TestContext.ComponentNames = new[] { "User" };
            TestContext.ComponentTypes = new[] { typeof(UserComponent) };
            _context = new TestContext();
        }

        [Fact]
        public void ProcessesEntity()
        {
            var system = new TestJobSystem(_context);
            var entity = _context.CreateEntity();
            entity.AddUser("Test", -1);
            system.Execute();
            entity.GetUser().Name.Should().Be("Test-Processed");
        }

        [Fact]
        public void ProcessesAllEntities()
        {
            var system = new TestJobSystem(_context);
            for (var i = 0; i < 4; i++)
                _context.CreateEntity().AddUser($"Test{i}", -1);

            system.Execute();

            var entities = _context.GetEntities();
            entities.Length.Should().Be(4);
            for (var i = 0; i < entities.Length; i++)
                entities[i].GetUser().Name.Should().Be($"Test{i}-Processed");
        }

        [Fact]
        public void ThrowsWhenThreadThrows()
        {
            var system = new TestJobSystem(_context);
            system.Exception = new Exception("Test Exception");
            for (var i = 0; i < 10; i++)
                _context.CreateEntity().AddUser($"Test{i}", -1);

            FluentActions.Invoking(() => system.Execute()).Should().Throw<Exception>();
        }

        [Fact]
        public void RecoversFromException()
        {
            var system = new TestJobSystem(_context);
            system.Exception = new Exception("Test Exception");
            for (var i = 0; i < 10; i++)
                _context.CreateEntity().AddUser($"Test{i}", -1);

            var didThrow = 0;
            try
            {
                system.Execute();
            }
            catch (Exception)
            {
                didThrow += 1;
            }

            didThrow.Should().Be(1);
            system.Exception = null;
            system.Execute();
        }
    }
}
