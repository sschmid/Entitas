using System.Linq;
using Entitas.CodeGeneration;
using Entitas.CodeGeneration.CodeGenerator;
using NSpec;

class describe_CodeGenerator : nspec {

    void when_generating() {

        context["generate"] = () => {

            it["executes data providers, generators and post processors"] = () => {
                var generator = new CodeGenerator(
                    new[] { new Data_1_2_Provider() },
                    new[] { new DataFile1CodeGenerator() },
                    new[] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.should_be(2);

                files[0].fileName.should_be("Test1File0-Processed1");
                files[0].fileContent.should_be("data1");

                files[1].fileName.should_be("Test1File1-Processed1");
                files[1].fileContent.should_be("data2");
            };

            it["uses returned CodeGenFiles"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new ICodeGenFilePostProcessor[] { new Processed1PostProcessor(), new NoFilesPostProcessor() }
                );

                var files = generator.Generate();

                files.Length.should_be(1);

                files[0].fileName.should_be("Test1File0-Processed1");
            };
        };

        context["dry run"] = () => {

            it["skips plugins which don't run in dry run"] = () => {
                var generator = new CodeGenerator(
                    new ICodeGeneratorDataProvider[] { new Data_1_2_Provider(), new DisabledDataProvider() },
                    new ICodeGenerator[] { new DataFile1CodeGenerator(), new DisabledCodeGenerator() },
                    new ICodeGenFilePostProcessor[] { new Processed1PostProcessor(), new DisabledPostProcessor() }
                );

                var files = generator.DryRun();

                files.Length.should_be(2);

                files[0].fileName.should_be("Test1File0-Processed1");
                files[1].fileName.should_be("Test1File1-Processed1");
            };
        };

        context["priority"] = () => {

            it["runs data provider based on priority"] = () => {
                var generator = new CodeGenerator(
                    new ICodeGeneratorDataProvider[] { new Data_3_4_Provider(), new Data_1_2_Provider() },
                    new[] { new DataFile1CodeGenerator() },
                    new[] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.should_be(4);

                files[0].fileName.should_be("Test1File0-Processed1");
                files[0].fileContent.should_be("data1");

                files[1].fileName.should_be("Test1File1-Processed1");
                files[1].fileContent.should_be("data2");

                files[2].fileName.should_be("Test1File2-Processed1");
                files[2].fileContent.should_be("data3");

                files[3].fileName.should_be("Test1File3-Processed1");
                files[3].fileContent.should_be("data4");
            };

            it["runs code generators based on priority"] = () => {
                var generator = new CodeGenerator(
                    new[] { new Data_1_2_Provider() },
                    new ICodeGenerator[] { new DataFile2CodeGenerator(), new DataFile1CodeGenerator() },
                    new[] { new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.should_be(4);

                files[0].fileName.should_be("Test1File0-Processed1");
                files[1].fileName.should_be("Test1File1-Processed1");
                files[2].fileName.should_be("Test2File0-Processed1");
                files[3].fileName.should_be("Test2File1-Processed1");
            };

            it["runs post processors based on priority"] = () => {
                var generator = new CodeGenerator(
                    new[] { new Data_1_2_Provider() },
                    new[] { new DataFile1CodeGenerator() },
                    new ICodeGenFilePostProcessor[] { new Processed2PostProcessor(), new Processed1PostProcessor() }
                );

                var files = generator.Generate();

                files.Length.should_be(2);

                files[0].fileName.should_be("Test1File0-Processed1-Processed2");
                files[1].fileName.should_be("Test1File1-Processed1-Processed2");
            };
        };

        context["cancel"] = () => {
            
            it["cancels"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                generator.OnProgress += (title, info, progress) => generator.Cancel();

                var files = generator.Generate();

                files.Length.should_be(0);
            };

            it["cancels dry run"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                generator.OnProgress += (title, info, progress) => generator.Cancel();

                var files = generator.DryRun();

                files.Length.should_be(0);
            };

            it["can generate again after cancel"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                GeneratorProgress onProgress = (title, info, progress) => generator.Cancel();
                generator.OnProgress += onProgress;

                generator.Generate();

                generator.OnProgress -= onProgress;

                var files = generator.Generate();

                files.Length.should_be(2);
            };

            it["can do dry run after cancel"] = () => {
                var generator = new CodeGenerator(
                    new [] { new Data_1_2_Provider() },
                    new [] { new DataFile1CodeGenerator() },
                    new [] { new Processed1PostProcessor() }
                );

                GeneratorProgress onProgress = (title, info, progress) => generator.Cancel();
                generator.OnProgress += onProgress;

                generator.Generate();

                generator.OnProgress -= onProgress;

                var files = generator.DryRun();

                files.Length.should_be(2);
            };
        };
    }
}

public class Data_1_2_Provider : ICodeGeneratorDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data1");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data2");

        return new[] {
            data1,
            data2
        };
    }
}

public class Data_3_4_Provider : ICodeGeneratorDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data3");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data4");

        return new[] {
            data1,
            data2
        };
    }
}

public class DisabledDataProvider : ICodeGeneratorDataProvider {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return false; } }

    public CodeGeneratorData[] GetData() {
        var data1 = new CodeGeneratorData();
        data1.Add("testKey", "data5");

        var data2 = new CodeGeneratorData();
        data2.Add("testKey", "data6");

        return new[] {
            data1,
            data2
        };
    }
}

public class DataFile1CodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test1File" + i,
                d["testKey"].ToString(),
                "Test1CodeGenerator"
            )).ToArray();
    }
}

public class DataFile2CodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test2File" + i,
                d["testKey"].ToString(),
                "Test2CodeGenerator"
            )).ToArray();
    }
}

public class DisabledCodeGenerator : ICodeGenerator {

    public string name { get { return ""; } }
    public int priority { get { return -5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return false; } }

    public CodeGenFile[] Generate(CodeGeneratorData[] data) {
        return data
            .Select((d, i) => new CodeGenFile(
                "Test3File" + i,
                d["testKey"].ToString(),
                "DisabledCodeGenerator"
            )).ToArray();
    }
}

public class Processed1PostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 0; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach(var file in files) {
            file.fileName += "-Processed1";
        }

        return files;
    }
}

public class Processed2PostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach(var file in files) {
            file.fileName += "-Processed2";
        }

        return files;
    }
}

public class DisabledPostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return 5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return false; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        foreach(var file in files) {
            file.fileName += "-Disabled";
        }

        return files;
    }
}

public class NoFilesPostProcessor : ICodeGenFilePostProcessor {

    public string name { get { return ""; } }
    public int priority { get { return -5; } }
    public bool isEnabledByDefault { get { return true; } }
    public bool runInDryMode { get { return true; } }

    public CodeGenFile[] PostProcess(CodeGenFile[] files) {
        return new[] { files[0] };
    }
}
