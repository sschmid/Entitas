using NSpec;

public class describe_Tests : nspec {
    void it_works() {
        true.should_be_true();
    }

    void it_fails() {
        true.should_be_false();
    }
}

