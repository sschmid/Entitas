using Entitas;
using UnityEngine;

public class RandomValueSystem : IExecuteSystem {

    readonly GameContext _context;

    public RandomValueSystem(Contexts contexts) {
        _context = contexts.game;
    }

    public void Execute() {
        _context.CreateEntity().AddMyFloat(Random.value);
    }
}
