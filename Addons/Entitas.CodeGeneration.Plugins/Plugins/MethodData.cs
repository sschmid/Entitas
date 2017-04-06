namespace Entitas.CodeGeneration.Plugins {

    public class MethodData {

        public readonly string returnType;
        public readonly string methodName;
        public readonly MemberData[] parameters;

        public MethodData(string returnType, string methodName, MemberData[] parameters) {
            this.returnType = returnType;
            this.methodName = methodName;
            this.parameters = parameters;
        }
    }
}
