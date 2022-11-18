namespace Entitas.Plugins
{
    public class MethodData
    {
        public readonly string ReturnType;
        public readonly string MethodName;
        public readonly MemberData[] Parameters;

        public MethodData(string returnType, string methodName, MemberData[] parameters)
        {
            ReturnType = returnType;
            MethodName = methodName;
            Parameters = parameters;
        }
    }
}
