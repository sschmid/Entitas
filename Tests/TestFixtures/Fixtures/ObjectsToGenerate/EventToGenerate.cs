using Entitas.CodeGeneration.Attributes;

namespace My.Namespace {

    [Context("Test"), Context("Test2"), Event(EventTarget.Any)]
    public sealed class EventToGenerate {
        public string value;
    }
}
