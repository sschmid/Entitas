using Entitas;
using Entitas.CodeGenerator;

[Context("Test"), SingleEntity, CustomPrefix("My")]
public class CustomPrefixComponent : IComponent {
}
