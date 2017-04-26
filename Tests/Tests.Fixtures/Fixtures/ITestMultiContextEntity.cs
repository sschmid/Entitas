using Entitas;

public interface ITestMultiContextEntity : IEntity {

    NameAgeComponent nameAge { get; }
}

public partial class TestEntity : ITestMultiContextEntity { }
public partial class Test2Entity : ITestMultiContextEntity { }