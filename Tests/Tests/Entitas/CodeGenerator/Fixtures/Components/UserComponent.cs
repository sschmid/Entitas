using System;
using Entitas.Api;
using Entitas.CodeGenerator;

[Context("Test"), Unique]
public class UserComponent : IComponent {

    public DateTime timestamp;
    public bool isLoggedIn;
}
