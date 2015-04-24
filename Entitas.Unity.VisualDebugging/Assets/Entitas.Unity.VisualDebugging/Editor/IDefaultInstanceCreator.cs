using System;

public interface IDefaultInstanceCreator {
    bool HandlesType(Type type);
    object CreateDefault(Type type);
}

