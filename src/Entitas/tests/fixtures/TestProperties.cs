using DesperateDevs.Serialization;

public class TestPreferences : Preferences {

    public TestPreferences(string properties, string userProperties = null)
        : base(new Properties(properties), new Properties(userProperties ?? string.Empty)) {
    }
}
