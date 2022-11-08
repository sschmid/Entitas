using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class GroupAllocController : MonoBehaviour
{
    public GroupAlloc Mode;
    public int Count;

    readonly List<GameEntity> _buffer = new List<GameEntity>();
    GameContext _context;
    IGroup<GameEntity> _group;

    void Start()
    {
        _context = Contexts.sharedInstance.game;
        _group = _context.GetGroup(GameMatcher.MyInt);
    }

    void Update()
    {
        _context.DestroyAllEntities();
        for (var i = 0; i < Count; i++)
            _context.CreateEntity().AddMyInt(i);

        Iterate();
    }

    void Iterate()
    {
        switch (Mode)
        {
            case GroupAlloc.Group:
                foreach (var e in _group)
                {
                    var unused = e.myInt.Value;
                }

                break;
            case GroupAlloc.GetEntities:
                foreach (var e in _group.GetEntities())
                {
                    var unused = e.myInt.Value;
                }

                break;
            case GroupAlloc.GetEntitiesBuffer:
                foreach (var e in _group.GetEntities(_buffer))
                {
                    var unused = e.myInt.Value;
                }

                break;
        }
    }
}

public enum GroupAlloc
{
    Group,
    GetEntities,
    GetEntitiesBuffer
}
