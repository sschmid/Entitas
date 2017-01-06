using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SomeAttribute : Attribute {

    public readonly string value;

    public SomeAttribute(string value) {
        this.value = value;
    }
}
