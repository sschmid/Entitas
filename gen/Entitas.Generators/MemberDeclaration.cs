namespace Entitas.Generators;

public readonly struct MemberDeclaration : IEquatable<MemberDeclaration>
{
    public readonly string Type;
    public readonly string Name;

    public MemberDeclaration(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public bool Equals(MemberDeclaration other) => Type == other.Type && Name == other.Name;
    public override bool Equals(object? obj) => obj is MemberDeclaration other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Type, Name);
}
