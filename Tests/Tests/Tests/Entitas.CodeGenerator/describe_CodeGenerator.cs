using System.Linq;
using Entitas.CodeGenerator;
using NSpec;

class describe_CodeGenerator : nspec {

    void when_generating() {

        it["executes data providers, generators and post processors"] = () => {
            var generator = new CodeGenerator(
                new [] { new TestDataProvider() },
                new [] { new TestCodeGenerator() },
                new [] { new TestPostProcessor() }
            );

            var files = generator.Generate();

            files.Length.should_be(2);

            files[0].fileName.should_be("FileName0-Approved!");
            files[1].fileName.should_be("FileName1-Approved!");
        };

        it["performs a dry run (no post processors)"] = () => {
            var generator = new CodeGenerator(
                new [] { new TestDataProvider() },
                new [] { new TestCodeGenerator() },
                new [] { new TestPostProcessor() }
            );

            var files = generator.DryRun();

            files.Length.should_be(2);

            files[0].fileName.should_be("FileName0");
            files[1].fileName.should_be("FileName1");
        };

        it["runs post processors based on priority"] = () => {
            var generator = new CodeGenerator(
                new [] { new TestDataProvider() },
                new [] { new TestCodeGenerator() },
                new ICodeGenFilePostProcessor[] { new TestPostProcessor(), new Test2PostProcessor() }
            );

            var files = generator.Generate();

            files.Length.should_be(2);

            files[0].fileName.should_be("FileName0First!-Approved!");
            files[1].fileName.should_be("FileName1First!-Approved!");
        };

        it["uses returned CodeGenFiles"] = () => {
            var generator = new CodeGenerator(
                new [] { new TestDataProvider() },
                new [] { new TestCodeGenerator() },
                new ICodeGenFilePostProcessor[] { new Test3PostProcessor(), new TestPostProcessor() }
            );

            var files = generator.Generate();

            files.Length.should_be(1);

            files[0].fileName.should_be("FileName0-Approved!");
        };
    }
}

public class TestDataProvider : ICodeGeneratorDataProvider {

    public string name { get { return ""; } }
    public bool isEnabledByDefault { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "value1");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "value2");

        return new [] {
            data1,
            data2
        };
    }
}

public class TestCodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public bool isEnabledByDefault { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "FileName" + i,
                d["testKey"].ToString(),
                "TestCodeGenerator"
            )).ToArray();
    }
}

public class TestPostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public bool isEnabledByDefault { get { return true; } }
    public int priority { get { return 10; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach(var file in files) {
            file.fileName += "-Approved!";
        }

        return files;
    }
}

public class Test2PostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public bool isEnabledByDefault { get { return true; } }
    public int priority { get { return 0; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach(var file in files) {
            file.fileName += "First!";
        }

        return files;
    }
}

public class Test3PostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public bool isEnabledByDefault { get { return true; } }
    public int priority { get { return 0; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        return new [] { files[0] };
    }
}
