using Entitas;
using Entitas.Api;
using Entitas.CodeGenerator;

[Context("Test"), Unique, CustomPrefix("My")]
public class CustomPrefixComponent : IComponent {
}
