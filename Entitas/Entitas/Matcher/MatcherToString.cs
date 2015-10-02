using System.Text;

namespace Entitas {
    public partial class Matcher {
        string _toStringCache;

        public override string ToString() {
            if (_toStringCache == null) {
                var sb = new StringBuilder();
                if (_allOfIndices != null) {
                    appendIndices(sb, "AllOf", _allOfIndices);
                }
                if (_anyOfIndices != null) {
                    if (_allOfIndices != null) {
                        sb.Append(".");
                    }
                    appendIndices(sb, "AnyOf", _anyOfIndices);
                }
                if (_noneOfIndices != null) {
                    appendIndices(sb, ".NoneOf", _noneOfIndices);
                }
                _toStringCache = sb.ToString();
            }

            return _toStringCache;
        }

        static void appendIndices(StringBuilder sb, string prefix, int[] indexArray) {
            const string seperator = ", ";
            sb.Append(prefix);
            sb.Append("(");
            var lastSeperator = indexArray.Length - 1;
            for (int i = 0, indicesLength = indexArray.Length; i < indicesLength; i++) {
                sb.Append(indexArray[i]);
                if (i < lastSeperator) {
                    sb.Append(seperator);
                }
            }
            sb.Append(")");
        }
    }
}
