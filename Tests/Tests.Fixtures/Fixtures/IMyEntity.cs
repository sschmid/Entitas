public interface IMyEntity : Entitas.IEntity, INameAge { }

public partial class TestEntity : IMyEntity { }
public partial class Test2Entity : IMyEntity { }