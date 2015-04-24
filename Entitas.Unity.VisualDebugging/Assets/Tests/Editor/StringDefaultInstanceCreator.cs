using System;

public class StringDefaultInstanceCreator : IDefaultInstanceCreator {
    public bool HandlesType(Type type) {
        return type == typeof(string);
    }

    public object CreateDefault(Type type) {
        return string.Empty;
    }
}

