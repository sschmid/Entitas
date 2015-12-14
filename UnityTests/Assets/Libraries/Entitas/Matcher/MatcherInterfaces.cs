namespace Entitas {
    public interface ICompoundMatcher : IMatcher {
        int[] allOfIndices { get; }
        int[] anyOfIndices { get; }
        int[] noneOfIndices { get; }
    }

    public interface IAllOfMatcher : ICompoundMatcher {
        IAnyOfMatcher AnyOf(params int[] indices);
        IAnyOfMatcher AnyOf(params IMatcher[] matchers);
        INoneOfMatcher NoneOf(params int[] indices);
        INoneOfMatcher NoneOf(params IMatcher[] matchers);
    }

    public interface IAnyOfMatcher : ICompoundMatcher {
        INoneOfMatcher NoneOf(params int[] indices);
        INoneOfMatcher NoneOf(params IMatcher[] matchers);
    }

    public interface INoneOfMatcher : ICompoundMatcher {
    }
}

