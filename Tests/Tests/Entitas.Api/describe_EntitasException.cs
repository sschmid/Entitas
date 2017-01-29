using Entitas.Api;
using NSpec;

class describe_EntitasException : nspec {

    void when_exception() {

        it["creates exception with hint separated by newLine"] = () => {
            const string msg = "Message";
            const string hint = "Hint";
            var ex = new EntitasException(msg, hint);
            ex.Message.should_be(msg + "\n" + hint);
        };

        it["ignores hint when null"] = () => {
            const string msg = "Message";
            string hint = null;
            var ex = new EntitasException(msg, hint);
            ex.Message.should_be(msg);
        };
    }
}
