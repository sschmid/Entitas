using System.Collections.Generic;

namespace Entitas {
    public abstract class AbstractCompoundMatcher : AbstractMatcher {
        public IMatcher[] matchers { get { return _matchers; } }

        readonly IMatcher[] _matchers;

        protected AbstractCompoundMatcher(IMatcher[] matchers) : base(extractIndices(matchers)) {
            _matchers = matchers;
        }

        static int[] extractIndices(IMatcher[] matchers) {
            var indices = new List<int>();
            for (int i = 0, matchersLength = matchers.Length; i < matchersLength; i++) {
                indices.AddRange(matchers[i].indices);
            }

            return indices.ToArray();
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode()) {
                return false;
            }

            var matcher = (AbstractCompoundMatcher)obj;
            if (matcher.matchers.Length != _matchers.Length) {
                return false;
            }

            for (int i = 0, matchersLength = matcher._matchers.Length; i < matchersLength; i++) {
                if (!matcher.matchers[i].Equals(_matchers[i])) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            if (_toStringCache == null) {
                const string seperator = ", ";
                var matcherStr = string.Empty;
                for (int i = 0, _matchersLength = _matchers.Length; i < _matchersLength; i++) {
                    matcherStr += _matchers[i] + seperator;
                }

                if (matcherStr != string.Empty) {
                    matcherStr = matcherStr.Substring(0, matcherStr.Length - seperator.Length);
                }

                var name = GetType().Name;
                if (name.EndsWith("CompoundMatcher")) {
                    name = name.Replace("CompoundMatcher", string.Empty);
                }
                _toStringCache =  name + "(" + matcherStr + ")";
            }

            return _toStringCache;
        }
    }
}

