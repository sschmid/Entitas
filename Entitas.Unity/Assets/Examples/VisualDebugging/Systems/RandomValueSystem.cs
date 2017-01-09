using Entitas;
using UnityEngine;

public class RandomValueSystem : IExecuteSystem {

    readonly Context _context;

    public RandomValueSystem(Contexts contexts) {
        _context = contexts.visualDebugging;
    }

    public void Execute() {
        _context.CreateEntity().AddMyFloat(Random.value);
    }
}
