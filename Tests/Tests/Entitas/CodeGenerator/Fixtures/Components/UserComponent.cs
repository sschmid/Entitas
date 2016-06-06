using System;
using Entitas;
using Entitas.CodeGenerator;

[SingleEntity]
public class UserComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
}
