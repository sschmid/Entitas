using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Entitas.Generators;

public readonly struct EntitasAnalyzerConfigOptions : IEquatable<EntitasAnalyzerConfigOptions>
{
    public const string TestValueKey = "entitas_test_option";

    public readonly bool TestValue;

    public EntitasAnalyzerConfigOptions(AnalyzerConfigOptions options)
    {
        TestValue = IsTrue(options, TestValueKey);
    }

    static bool IsTrue(AnalyzerConfigOptions options, string key)
    {
        return options.TryGetValue(key, out var value) && value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(EntitasAnalyzerConfigOptions other) => TestValue == other.TestValue;
    public override bool Equals(object? obj) => obj is EntitasAnalyzerConfigOptions other && Equals(other);
    public override int GetHashCode() => TestValue.GetHashCode();
}
