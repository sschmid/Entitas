using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TestMemberAttribute : Attribute
{
    public readonly string Value;

    public TestMemberAttribute(string value)
    {
        Value = value;
    }
}
