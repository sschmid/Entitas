namespace Entitas.CodeGenerator {

    public interface ICodeGeneratorInterface {

        string name { get; }
        bool isEnabledByDefault { get; }
    }
}
