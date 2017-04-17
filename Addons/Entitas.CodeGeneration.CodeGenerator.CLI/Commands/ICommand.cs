namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public interface ICommand {

        string trigger { get; }
        void Run(string[] args);
    }
}
