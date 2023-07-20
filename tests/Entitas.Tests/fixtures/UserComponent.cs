using Entitas;

public sealed class UserComponent : IComponent
{
    public string Name;
    public int Age;

    public override string ToString()
    {
        return $"User({Name}, {Age})";
    }
}

namespace My.Namespace
{
    public sealed class UserComponent : IComponent
    {
        public string Name;
        public int Age;
    }
}
