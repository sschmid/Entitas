using Entitas;
using System.Linq;

public class EntityEmittingSubSystem : IReactiveSystem {
    public int didExecute { get { return _didExecute; } }

    public Entity[] entites { get { return _entites; } }

    Context _context;
    int _didExecute;
    Entity[] _entites;

    public EntityEmittingSubSystem(Context context) {
        _context = context;
    }

    public IMatcher GetTriggeringMatcher() {
        return Matcher.AllOf(new [] { CID.ComponentA });
    }

    public GroupEventType GetEventType() {
        return GroupEventType.OnEntityAdded;
    }

    public void Execute(Entity[] entities) {
        _context.CreateEntity().AddComponent(CID.ComponentA, new ComponentA());
        _entites = entities.ToArray();
        _didExecute++;
    }
}

