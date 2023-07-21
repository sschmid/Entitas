using MyApp;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class EntityIndexTests
    {
        [Fact]
        public void GeneratesEventSystems()
        {
            ContextInitialization.InitializeMain();
            new MainContext().AddAllEntityIndexes();
        }

        [Fact]
        public void GeneratesEmptyEventSystems()
        {
            ContextInitialization.InitializeEmpty();
            new EmptyContext().AddAllEntityIndexes();
        }
    }
}
