public class Contexts {


    // TODO

    public static Contexts sharedInstance { get; }

    public VisualDebuggingContext visualDebugging;
    public BlueprintsContext blueprints;


    public static VisualDebuggingContext CreateVisualDebuggingContext() { return null; }
    public static BlueprintsContext CreateBlueprintsContext() { return null; }
}
