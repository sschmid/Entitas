using System.Collections.Immutable;

namespace Entitas.Generators;

public readonly struct ComponentDeclaration : IEquatable<ComponentDeclaration>
{
    public readonly string? Namespace;
    public readonly string Type;
    public readonly string Name;
    public readonly ImmutableArray<MemberDeclaration> Members;

    public ComponentDeclaration(string? @namespace, string type, string name, ImmutableArray<MemberDeclaration> members)
    {
        Namespace = @namespace;
        Type = type;
        Name = name;
        Members = members;
    }

    public bool Equals(ComponentDeclaration other) =>
        Namespace == other.Namespace &&
        Type == other.Type &&
        Name == other.Name &&
        Members.SequenceEqual(other.Members);

    public override bool Equals(object? obj) => obj is ComponentDeclaration other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Namespace, Type, Name, Members);
}
