using System.Text;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        string _toStringCache;
        StringBuilder _toStringBuilder;

        public override string ToString()
        {
            if (_toStringCache == null)
            {
                _toStringBuilder ??= new StringBuilder();
                _toStringBuilder.Length = 0;

                if (_allOfIndexes != null)
                {
                    AppendIndexes(_toStringBuilder, "AllOf", _allOfIndexes, ComponentNames);
                }

                if (_anyOfIndexes != null)
                {
                    if (_allOfIndexes != null)
                        _toStringBuilder.Append(".");

                    AppendIndexes(_toStringBuilder, "AnyOf", _anyOfIndexes, ComponentNames);
                }

                if (_noneOfIndexes != null)
                {
                    AppendIndexes(_toStringBuilder, ".NoneOf", _noneOfIndexes, ComponentNames);
                }

                _toStringCache = _toStringBuilder.ToString();
            }

            return _toStringCache;
        }

        static void AppendIndexes(StringBuilder sb, string prefix, int[] indexArray, string[] componentNames)
        {
            const string separator = ", ";
            sb.Append(prefix);
            sb.Append("(");
            var lastSeparator = indexArray.Length - 1;
            for (var i = 0; i < indexArray.Length; i++)
            {
                var index = indexArray[i];
                if (componentNames == null)
                    sb.Append(index);
                else
                    sb.Append(componentNames[index]);

                if (i < lastSeparator)
                    sb.Append(separator);
            }

            sb.Append(")");
        }
    }
}
