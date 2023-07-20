using System;

namespace Entitas
{
    public class MatcherException : Exception
    {
        public MatcherException(int indices) :
            base($"Matcher.Indices.Length must be 1 but was {indices}") { }
    }
}
