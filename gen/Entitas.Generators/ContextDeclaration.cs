using Microsoft.CodeAnalysis;

namespace Entitas.Generators;

public readonly struct ContextDeclaration : IEquatable<ContextDeclaration>
{
    public readonly string? Namespace;
    public readonly string Type;
    public readonly string Name;

    public readonly Location Location;

    public readonly string ShortName;
    public readonly string EntityName;

    public ContextDeclaration(string? @namespace, string type, string name, Location location)
    {
        Namespace = @namespace;
        Type = type;
        Name = name;
        Location = location;

        ShortName = name.RemoveContextSuffix();
        EntityName = ShortName.AddEntitySuffix();
    }

    public bool Equals(ContextDeclaration other) =>
        Namespace == other.Namespace &&
        Type == other.Type &&
        Name == other.Name;

    public override bool Equals(object? obj) => obj is ContextDeclaration other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Namespace, Type, Name);
}
