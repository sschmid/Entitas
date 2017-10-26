using System.Collections.Generic;
using Entitas.Utils;
using NSpec;

class describe_Properties : nspec {

    void assertProperties(string input, string expectedOutput, Dictionary<string, string> expectedProperties, Properties properties = null) {
        var p = properties ?? new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.count.should_be(expectedCount);
        p.ToString().should_be(expectedOutput);
        if (expectedProperties != null) {
            foreach (var kv in expectedProperties) {
                p.HasKey(kv.Key).should_be_true();
                p[kv.Key].should_be(kv.Value);
            }
        }
    }

    void assertProperties(Dictionary<string, string> input, string expectedOutput, Dictionary<string, string> expectedProperties) {
        var p = new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.count.should_be(expectedCount);
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

        context["when invalid input string"] = () => {

            it["throws when invalid key"] = () => {
                var input =
                    "some.key.1 = some value 1" + "\n" +
                    "some.key.2" + "\n" +
                    "some.key.3 = some value 3" + "\n";

                InvalidKeyPropertiesException exception = null;
                try {
                    new Properties(input);
                } catch(InvalidKeyPropertiesException ex) {
                    exception = ex;
                }

                exception.should_not_be_null();
                exception.key.should_be("some.key.2");
            };
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

            it["removes whitespace after value"] = () => {
                const string input = "some.key = some value ";

                const string expectedOutput = "some.key = some value\n";
                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["when multiline"] = () => {

            it["creates Properties from multiline input string"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2" + "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2\n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2" },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["creates Properties from multiline input string where values contain ="] = () => {
                var input =
                    "some.key.1=some=value 1" + "\n" +
                    "some.key.2 ==some value 2" + "\n" +
                    "some.key.3=some value=" + "\n";

                const string expectedOutput =
                    "some.key.1 = some=value 1\n" +
                    "some.key.2 = =some value 2\n" +
                    "some.key.3 = some value=\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some=value 1" },
                    { "some.key.2", "=some value 2" },
                    { "some.key.3", "some value=" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores blank lines"] = () => {
                var input =
                    "\n" +
                    "some.key.1=some value 1" + "\n" +
                    "\n" +
                    " some.key.2 = some value 2" + "\n" +
                    "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.1 = some value 1\n" +
                    "some.key.2 = some value 2\n" +
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.1", "some value 1" },
                    { "some.key.2", "some value 2" },
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["ignores lines starting with #"] = () => {
                var input =
                    "#some.key.1=some value 1" + "\n" +
                    "  #some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                const string expectedOutput =
                    "some.key.3 = some value 3\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key.3", "some value 3" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["supports multiline values ending with \\"] = () => {
                var input =
                    "some.key=some val\\" + "\n" + "ue" + "\n" +
                    "some.other.key=other val\\" + "\n" + "ue" + "\n";

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
                    "some.key=some val\\" + "\n" + "   ue" + "\n" +
                    "some.other.key=other val\\" + "\n" + "   ue" + "\n";

                const string expectedOutput =
                    "some.key = some value\n" +
                    "some.other.key = other value\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "some.key", "some value" },
                    { "some.other.key", "other value" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["has all keys"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var keys = new Properties(input).keys;
                keys.Length.should_be(3);
                keys.should_contain("some.key.1");
                keys.should_contain("some.key.2");
                keys.should_contain("some.key.3");
            };

            it["has all values"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var values = new Properties(input).values;
                values.Length.should_be(3);
                values.should_contain("some value 1");
                values.should_contain("some value 2");
                values.should_contain("some value 3");
            };

            it["gets a dictionary"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var dict = new Properties(input).ToDictionary();
                dict.Count.should_be(3);
                dict.ContainsKey("some.key.1").should_be_true();
                dict.ContainsKey("some.key.2").should_be_true();
                dict.ContainsKey("some.key.3").should_be_true();
            };

            it["gets a dictionary copy"] = () => {
                var input =
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

                var properties = new Properties(input);
                properties.ToDictionary().should_not_be_same(properties.ToDictionary());
            };
        };

        context["when replacing special characters in values"] = () => {

            it["replaces \\n with newline"] = () => {
                var input =
                    @"some.key=some\nvalue" + "\n" +
                    @"some.other.key=other\nvalue" + "\n";

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
                    @"some.key=some\tvalue" + "\n" +
                    @"some.other.key=other\tvalue" + "\n";

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

            it["removes trailing whitespace of value"] = () => {
                p["key"] = "value";
                p["key"].should_be("value");
            };

            it["adds properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p.AddProperties(dict, true);

                p.count.should_be(dict.Count);
                p["key1"].should_be("value1");
                p["key2"].should_be("value2");
            };

            it["overwrites existing properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p["key1"] = "existingKey";
                p.AddProperties(dict, true);

                p.count.should_be(dict.Count);
                p["key1"].should_be("value1");
                p["key2"].should_be("value2");
            };

            it["only adds missing properties from dictionary"] = () => {
                var dict = new Dictionary<string, string> {
                    { "key1", "value1"},
                    { "key2", "value2"}
                };

                p["key1"] = "existingKey";
                p.AddProperties(dict, false);

                p.count.should_be(dict.Count);
                p["key1"].should_be("existingKey");
                p["key2"].should_be("value2");
            };
        };

        context["removing properties"] = () => {

            Properties p = null;

            before = () => {
                p = new Properties();
                p["key"] = "value";
            };

            it["set new property"] = () => {
                p.RemoveProperty("key");
                p.HasKey("key").should_be_false();
            };
        };

        context["placeholder"] = () => {

            it["replaces placeholder within ${...}"] = () => {
                var input =
                    "project.name = Entitas" + "\n" +
                    "project.domain = com.sschmid" + "\n" +
                    "project.bundleId = ${project.domain}.${project.name}" + "\n";

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Entitas" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };

            it["replaces placeholder when adding new property"] = () => {
                var input =
                    "project.name = Entitas" + "\n" +
                    "project.domain = com.sschmid" + "\n";

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Entitas" }
                };

                var p = new Properties(input);
                p["project.bundleId"] = "${project.domain}.${project.name}";

                assertProperties(input, expectedOutput, expectedProperties, p);
            };

            it["doesn't replace placeholder when not resolvable"] = () => {
                var input =
                    "project.name = Entitas" + "\n" +
                    "project.domain = com.sschmid" + "\n" +
                    "project.bundleId = ${Xproject.domain}.${Xproject.name}" + "\n";

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${Xproject.domain}.${Xproject.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "${Xproject.domain}.${Xproject.name}" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["different line endings"] = () => {

            it["converts and normalizes line endings"] = () => {
                var input =
                    "project.name = Entitas" + "\n" +
                    "project.domain = com.sschmid" + "\r" +
                    "project.bundleId = ${project.domain}.${project.name}" + "\r\n";

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Entitas" }
                };

                assertProperties(input, expectedOutput, expectedProperties);
            };
        };

        context["minified string"] = () => {

            it["puts values in one line"] = () => {
                var properties = new Properties(
@"key = value1, value2, value3
key2 = value4");

                properties.ToMinifiedString().should_be(
@"key = value1, value2, value3
key2 = value4
");
            };
        };
    }

    void when_creating_properties_from_dictionary() {

        it["creates properties from dictionary"] = () => {
            var input = new Dictionary<string, string> {
                { "key1", "value1"},
                { "key2", "value2"}
            };

            assertProperties(
                input,
                "key1 = value1" + "\n" +
                "key2 = value2" + "\n",
                input
            );
        };

        it["uses copy of original dictionary"] = () => {
            var input = new Dictionary<string, string> {
                { "key1", "value1"},
                { "key2", "value2"}
            };

            var p = new Properties(input);
            p["key1"] = "newValue1";

            input["key1"].should_be("value1");
            p["key1"].should_be("newValue1");
        };

        context["placeholder"] = () => {

            it["replaces placeholder within ${...}"] = () => {
                var input = new Dictionary<string, string> {
                    { "project.name", "Entitas"},
                    { "project.domain", "com.sschmid"},
                    { "project.bundleId", "${project.domain}.${project.name}"}
                };

                const string expectedOutput =
                    "project.name = Entitas\n" +
                    "project.domain = com.sschmid\n" +
                    "project.bundleId = ${project.domain}.${project.name}\n";

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
