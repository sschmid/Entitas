namespace Entitas.CodeGenerator {

    public interface ICodeGeneratorInterface {

        string name { get; }
        int priority { get; }
        bool isEnabledByDefault { get; }
        bool runInDryMode { get; }
    }
}
