using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TestMemberAttribute : Attribute
{
    public readonly string value;

    public TestMemberAttribute(string value)
    {
        this.value = value;
    }
}
