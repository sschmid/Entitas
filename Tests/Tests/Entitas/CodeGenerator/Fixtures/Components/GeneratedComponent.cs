using Entitas;
using Entitas.CodeGenerator;

[Pool("ServicePool"), CustomComponentName("GeneratedService")]
public class SomeService {
}

[Pool("ServicePool")]
public class GeneratedService : IComponent {
    public SomeService value;
}

