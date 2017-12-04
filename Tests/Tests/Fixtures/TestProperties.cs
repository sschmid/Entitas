using DesperateDevs.Serialization;

public class TestPreferences : Preferences {

    public TestPreferences(string properties, string userPoperties = null)
        : base(new Properties(properties), new Properties(userPoperties ?? string.Empty)) {
    }
}
