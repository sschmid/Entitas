using System;
using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, Context("Test")]
public class UserComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
}
