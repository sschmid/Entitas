using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Plugins.Attributes;

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
                for (var x = position.X; x < position.X + size.Width; x++)
                for (var y = position.Y; y < position.Y + size.Height; y++)
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
    public int X;
    public int Y;
}

[Test1]
public class SizeComponent : IComponent
{
    public int Width;
    public int Height;
}

public struct IntVector2 : IEquatable<IntVector2>
{
    public int X;
    public int Y;

    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(IntVector2 other) => other.X == X && other.Y == Y;
}
