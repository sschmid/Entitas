using System;
using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, Pool("Other")]
public class OtherPoolComponent : IComponent {
    public DateTime timestamp;
    public bool isLoggedIn;
}
