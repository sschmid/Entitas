using System;
using Entitas;
using Entitas.CodeGenerator;

[Context("Test"), SingleEntity]
public class UserComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
}
