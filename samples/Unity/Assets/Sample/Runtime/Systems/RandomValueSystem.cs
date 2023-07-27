using Entitas;
using UnityEngine;

public class RandomValueSystem : IExecuteSystem
{
    readonly GameContext _context;

    public RandomValueSystem(GameContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        _context.CreateEntity().AddMyFloat(Random.value);
    }
}
