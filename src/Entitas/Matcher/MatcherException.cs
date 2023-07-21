using System;

namespace Entitas
{
    public class MatcherException : Exception
    {
        public MatcherException(int indexes) : base($"Matcher.Indexes.Length must be 1 but was {indexes}") { }
    }
}
