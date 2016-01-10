using System.Text;

namespace Entitas {
    public partial class Matcher {

        public string[] componentNames;

        string _toStringCache;

        public override string ToString() {
            if (_toStringCache == null) {
                var sb = new StringBuilder();
                if (_allOfIndices != null) {
                    appendIndices(sb, "AllOf", _allOfIndices, componentNames);
                }
                if (_anyOfIndices != null) {
                    if (_allOfIndices != null) {
                        sb.Append(".");
                    }
                    appendIndices(sb, "AnyOf", _anyOfIndices, componentNames);
                }
                if (_noneOfIndices != null) {
                    appendIndices(sb, ".NoneOf", _noneOfIndices, componentNames);
                }
                _toStringCache = sb.ToString();
            }

            return _toStringCache;
        }

        static void appendIndices(StringBuilder sb, string prefix, int[] indexArray, string[] componentNames) {
            const string separator = ", ";
            sb.Append(prefix);
            sb.Append("(");
            var lastSeparator = indexArray.Length - 1;
            for (int i = 0, indicesLength = indexArray.Length; i < indicesLength; i++) {
                var index = indexArray[i];
                if (componentNames == null) {
                    sb.Append(index);
                } else {
                    sb.Append(componentNames[index]);
                }

                if (i < lastSeparator) {
                    sb.Append(separator);
                }
            }
            sb.Append(")");
        }
    }
}
