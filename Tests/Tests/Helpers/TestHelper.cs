using NSpec;

public static class TestHelper {
    public static void Fail() {
        true.should_be_false();
    }
}

