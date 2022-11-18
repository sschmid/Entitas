using Entitas.Plugins.Attributes;

public abstract class AbstractEntityIndexComponent
{
    [EntityIndex]
    public string value;
}
