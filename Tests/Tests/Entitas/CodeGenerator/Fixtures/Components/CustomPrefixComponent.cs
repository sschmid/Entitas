using Entitas;
using Entitas.CodeGenerator;

[SingleEntity, CustomPrefix("My"), Context("Test")]
public class CustomPrefixComponent : IComponent {
}
