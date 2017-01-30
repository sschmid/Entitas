using Entitas.CodeGenerator;
using NSpec;

class describe_CodeGenFile : nspec {

    void when_creating() {

        it["set fields"] = () => {
            var file = new CodeGenFile("name.cs", "content", "MyGenerator");
            file.fileName.should_be("name.cs");
            file.fileContent.should_be("content");
            file.generatorName.should_be("MyGenerator");
        };

        it["converts new lines to unix"] = () => {
            var file = new CodeGenFile(null, "line1\r\nline2\r", null);
            file.fileContent.should_be("line1\nline2\n");
        };
    }
}
