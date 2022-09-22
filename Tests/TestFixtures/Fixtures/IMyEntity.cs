public interface IMyEntity : Entitas.IEntity, INameAgeEntity { }

public partial class TestEntity : IMyEntity { }

public partial class Test2Entity : IMyEntity { }
