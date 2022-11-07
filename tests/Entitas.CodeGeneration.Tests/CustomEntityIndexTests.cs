using System.Linq;
using FluentAssertions;
using MyNamespace;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class CustomEntityIndexTests
    {
        [Fact]
        public void SetsEntityToMultipleKeys()
        {
            var context = new TestContext();
            var index = new CustomEntityIndex(context);
            var e = context.CreateEntity();
            e.AddPosition(2, 3);
            e.AddSize(2, 2);

            index.GetEntities(new IntVector2(2, 3)).Single().Should().BeSameAs(e);
            index.GetEntities(new IntVector2(3, 3)).Single().Should().BeSameAs(e);
            index.GetEntities(new IntVector2(2, 4)).Single().Should().BeSameAs(e);
            index.GetEntities(new IntVector2(3, 4)).Single().Should().BeSameAs(e);
        }
    }
}
