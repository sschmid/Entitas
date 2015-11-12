using System.Text;

namespace Entitas {
    public partial class Matcher {

        public delegate string ComponentIndexResolver(int index);

        public ComponentIndexResolver componentIndexResolver;

        string _toStringCache;

        public override string ToString() {
            if (_toStringCache == null) {
                var sb = new StringBuilder();
                if (_allOfIndices != null) {
                    appendIndices(sb, "AllOf", _allOfIndices, componentIndexResolver);
                }
                if (_anyOfIndices != null) {
                    if (_allOfIndices != null) {
                        sb.Append(".");
                    }
                    appendIndices(sb, "AnyOf", _anyOfIndices, componentIndexResolver);
                }
                if (_noneOfIndices != null) {
                    appendIndices(sb, ".NoneOf", _noneOfIndices, componentIndexResolver);
                }
                _toStringCache = sb.ToString();
            }

            return _toStringCache;
        }

        static void appendIndices(StringBuilder sb, string prefix, int[] indexArray, ComponentIndexResolver componentIndexResolver) {
            const string SEPARATOR = ", ";
            sb.Append(prefix);
            sb.Append("(");
            var lastSeparator = indexArray.Length - 1;
            for (int i = 0, indicesLength = indexArray.Length; i < indicesLength; i++) {
                var index = indexArray[i];
                if (componentIndexResolver == null) {
                    sb.Append(index);
                } else {
                    sb.Append(componentIndexResolver(index));
                }

                if (i < lastSeparator) {
                    sb.Append(SEPARATOR);
                }
            }
            sb.Append(")");
        }
    }
}
