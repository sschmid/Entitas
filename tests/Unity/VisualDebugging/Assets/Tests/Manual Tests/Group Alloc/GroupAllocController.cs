using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class GroupAllocController : MonoBehaviour {

    public GroupAlloc mode;
    public int count;

    GameContext _context;
    IGroup<GameEntity> _group;
    List<GameEntity> _buffer = new List<GameEntity>();

    void Start() {
        _context = Contexts.sharedInstance.game;
        _group = _context.GetGroup(GameMatcher.MyInt);
    }

    void Update() {

        _context.DestroyAllEntities();
        for (int i = 0; i < count; i++) {
            _context.CreateEntity().AddMyInt(i);
        }

        iterate();
    }

    void iterate() {
        switch (mode) {
            case GroupAlloc.Group:
                foreach (var e in _group) {
                    var i = e.myInt.myInt;
                }

                break;
            case GroupAlloc.GetEntities:
                foreach (var e in _group.GetEntities()) {
                    var i = e.myInt.myInt;
                }

                break;
            case GroupAlloc.GetEntitiesBuffer:
                foreach (var e in _group.GetEntities(_buffer)) {
                    var i = e.myInt.myInt;
                }

                break;
        }
    }
}

public enum GroupAlloc {
    Group,
    GetEntities,
    GetEntitiesBuffer
}
