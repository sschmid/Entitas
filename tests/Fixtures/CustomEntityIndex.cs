using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace MyNamespace
{
    [CustomEntityIndex(typeof(Test1Context))]
    public class CustomEntityIndex : EntityIndex<Test1Entity, IntVector2>
    {
        static readonly List<IntVector2> _cachedList = new List<IntVector2>();

        public CustomEntityIndex(Test1Context context) : base(
            "MyCustomEntityIndex",
            context.GetGroup(Matcher<Test1Entity>.AllOf(Test1Matcher.Position, Test1Matcher.Size)),
            (e, c) =>
            {
                var position = c as PositionComponent ?? e.position;
                var size = c as SizeComponent ?? e.size;
                _cachedList.Clear();
                for (var x = position.x; x < position.x + size.width; x++)
                for (var y = position.y; y < position.y + size.height; y++)
                    _cachedList.Add(new IntVector2(x, y));

                return _cachedList.ToArray();
            }
        ) { }

        [EntityIndexGetMethod]
        public HashSet<Test1Entity> GetEntitiesWithPosition(IntVector2 position)
        {
            return GetEntities(position);
        }

        [EntityIndexGetMethod]
        public HashSet<Test1Entity> GetEntitiesWithPosition2(IntVector2 position, IntVector2 size)
        {
            return GetEntities(position);
        }
    }
}

[Test1]
public class PositionComponent : IComponent
{
    public int x;
    public int y;
}

[Test1]
public class SizeComponent : IComponent
{
    public int width;
    public int height;
}

public struct IntVector2 : IEquatable<IntVector2>
{
    public int x;
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(IntVector2 other) => other.x == x && other.y == y;
}
