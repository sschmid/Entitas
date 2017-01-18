using System;
using Entitas;
using Entitas.CodeGenerator;

[Context("Other"), SingleEntity]
public class OtherContextComponent : IComponent {

    public DateTime timestamp;
    public bool isLoggedIn;
}
