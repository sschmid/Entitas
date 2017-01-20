using Entitas;
using Entitas.Api;
using Entitas.CodeGenerator;

[Context("ServiceContext"), CustomComponentName("GeneratedService")]
public class SomeService {
}

[Context("ServiceContext")]
public class GeneratedService : IComponent {
    public SomeService value;
}
