using System;
using Entitas;
using Entitas.Api;
using Entitas.CodeGenerator;

[Context("Other"), Unique]
public class OtherContextComponent : IComponent {

    public DateTime timestamp;
    public bool isLoggedIn;
}
