using Entitas.CodeGeneration.CodeGenerator;

namespace Readme {

    class MainClass {

        public static void Main(string[] args) {

            // Configure code generator in Entitas.properties file
            CodeGeneratorUtil
                .CodeGeneratorFromConfig("../../Entitas.properties")
                .Generate();
        }
    }
}
