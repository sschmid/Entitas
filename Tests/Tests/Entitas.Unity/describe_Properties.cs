using System;
using System.Collections.Generic;
using Entitas.Unity;
using NSpec;

class describe_Properties : nspec {

    void assertProperties(string input, string expectedOutput, Dictionary<string, string>expectedProperties) {
        var p = new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.Count.should_be(expectedCount);
        p.ToString().should_be(expectedOutput);
        if (expectedProperties != null) {
            foreach (var kv in expectedProperties) {
                p.HasKey(kv.Key).should_be_true();
                p[kv.Key].should_be(kv.Value);
            }
        }
    }

    void when_creating_properties() {
        context["when empty"] = () => {
            it["is empty"] = () => assertProperties(string.Empty, string.Empty, null);
        };

        context["when single line"] = () => {
            it["creates Properties from single line input string"] = () => {
                const string input = "some.key=some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["ignores whitespace"] = () => {
            it["ignores whitespace between key and value"] = () => {
                const string input = "some.key  =  some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores whitespace before key"] = () => {
                const string input = "  some.key = some value";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["keeps whitespace after value"] = () => {
                const string input = "some.key = some value ";

                const string expectedOutput = "some.key = some value \n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value " }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["when multiline"] = () => {
            it["creates Properties from multiline input string"] = () => {
                var input =
                    "some.key.1=some value 1" + Environment.NewLine +
                    " some.key.2 = some value 2 " + Environment.NewLine +
                    "some.key.3=some value 3" + Environment.NewLine;

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2 \n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2 " },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["creates Properties from multiline input string where values contain ="] = () => {
                var input =
                    "some.key.1=some=value 1" + Environment.NewLine +
                    "some.key.2 ==some value 2 " + Environment.NewLine +
                    "some.key.3=some value=" + Environment.NewLine;

                const string expectedOutput =
                    "some.key.1 = some=value 1\n" +
                    "some.key.2 = =some value 2 \n" +
                    "some.key.3 = some value=\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some=value 1" },
                    { "some.key.2", "=some value 2 " },
                    { "some.key.3", "some value=" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores blank lines"] = () => {
                var input =
                    Environment.NewLine +
                    "some.key.1=some value 1" + Environment.NewLine +
                    Environment.NewLine +
                    " some.key.2 = some value 2 " + Environment.NewLine +
                    Environment.NewLine +
                    "some.key.3=some value 3" + Environment.NewLine;

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2 \n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2 " },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores lines starting with #"] = () => {
                var input =
                    "#some.key.1=some value 1" + Environment.NewLine +
                    "  #some.key.2 = some value 2 " + Environment.NewLine +
                    "some.key.3=some value 3" + Environment.NewLine;

                const string expectedOutput =
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["supports multiline values ending with \\"] = () => {
                var input =
                    "some.key=some val\\" + Environment.NewLine + "ue" + Environment.NewLine +
                    "some.other.key=other val\\" +Environment.NewLine + "ue" + Environment.NewLine;

                const string expectedOutput =
                    "some.key = some value\n" +
                    "some.other.key = other value\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" },
                    { "some.other.key", "other value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["trims leading whitespace of multilines"] = () => {
                var input =
                    "some.key=some val\\" + Environment.NewLine + "   ue" + Environment.NewLine +
                    "some.other.key=other val\\" + Environment.NewLine + "   ue" + Environment.NewLine;

                const string expectedOutput =
                    "some.key = some value\n" +
                    "some.other.key = other value\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" },
                    { "some.other.key", "other value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["when replacing special characters in values"] = () => {
            it["replaces \\n with newline"] = () => {
                var input =
                    @"some.key=some\nvalue" + Environment.NewLine +
                    @"some.other.key=other\nvalue" + Environment.NewLine;

                const string expectedOutput =
                    @"some.key = some\nvalue" + "\n" +
                    @"some.other.key = other\nvalue" + "\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some\nvalue" },
                    { "some.other.key", "other\nvalue" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["replaces \\t with tabs"] = () => {
                var input =
                    @"some.key=some\tvalue" + Environment.NewLine +
                    @"some.other.key=other\tvalue" + Environment.NewLine;

                const string expectedOutput =
                    @"some.key = some\tvalue" + "\n" +
                    @"some.other.key = other\tvalue" + "\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some\tvalue" },
                    { "some.other.key", "other\tvalue" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["adding properties"] = () => {
            Properties p = null;
            before = () => {
                p = new Properties();
            };

            it["set new property"] = () => {
                p["key"] = "value";
                p["key"].should_be("value");
            };

            it["trims key"] = () => {
                p[" key "] = "value";
                p["key"].should_be("value");
            };

            it["trims start of value"] = () => {
                p["key"] = " value";
                p["key"].should_be("value");
            };

            it["keeps trailing whitespace of value"] = () => {
                p["key"] = "value ";
                p["key"].should_be("value ");
            };
        };

        context["placeholder"] = () => {
            it["replaces placeholder within ${...}"] = () => {
                var input =
                    "project.name = Entitas" + Environment.NewLine +
                    "project.domain = com.sschmid" + Environment.NewLine +
                    "project.bundleId = ${project.domain}.${project.name}" + Environment.NewLine;

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = com.sschmid.Entitas\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Entitas" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };
    }
}

