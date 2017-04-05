using System.Collections.Generic;
using Entitas;
using NSpec;

class describe_Properties : nspec {

    void assertProperties(string input, string expectedOutput, Dictionary<string, string> expectedProperties) {
        var p = new Properties(input);
        var expectedCount = expectedProperties != null ? expectedProperties.Count : 0;
        p.count.should_be(expectedCount);
        p.ToString().should_be(expectedOutput);
        if(expectedProperties != null) {
            foreach(var kv in expectedProperties) {
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
                    "some.key.1=some value 1" + "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "some.key.3=some value 3" + "\n";

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
                    "some.key.1=some=value 1" + "\n" +
                    "some.key.2 ==some value 2 " + "\n" +
                    "some.key.3=some value=" + "\n";

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
                    "\n" +
                    "some.key.1=some value 1" + "\n" +
                    "\n" +
                    " some.key.2 = some value 2 " + "\n" +
                    "\n" +
                    "some.key.3=some value 3" + "\n";

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
                values.should_contain("some value 2 ");
                values.should_contain("some value 3");
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

            it["keeps trailing whitespace of value"] = () => {
                p["key"] = "value ";
                p["key"].should_be("value ");
            };
        };

        context["removing properties"] = () => {

            Properties p = null;

            before = () => {
                p = new Properties();
                p["key"] = "value";
            };

            it["set new property"] = () => {
                p.RemoveKey("key");
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
                    "project.bundleId = com.sschmid.Entitas\n";

                var expectedProperties = new Dictionary<string, string> {
                    { "project.name", "Entitas" },
                    { "project.domain", "com.sschmid" },
                    { "project.bundleId", "com.sschmid.Entitas" }
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
