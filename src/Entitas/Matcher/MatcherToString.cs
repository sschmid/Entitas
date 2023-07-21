using System.Linq;
using System.Text;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        string _toStringCache;

        public override string ToString()
        {
            if (_toStringCache == null)
            {
                var sb = new StringBuilder();
                if (_allOfIndexes != null)
                {
                    sb.Append(GetComponentNames("AllOf", _allOfIndexes, ComponentNames));
                }

                if (_anyOfIndexes != null)
                {
                    if (_allOfIndexes != null)
                        sb.Append(".");

                    sb.Append(GetComponentNames("AnyOf", _anyOfIndexes, ComponentNames));
                }

                if (_noneOfIndexes != null)
                {
                    sb.Append(GetComponentNames(".NoneOf", _noneOfIndexes, ComponentNames));
                }

                _toStringCache = sb.ToString();
            }

            return _toStringCache;
        }

        static string GetComponentNames(string prefix, int[] indexArray, string[] componentNames)
        {
            return componentNames != null
                ? $"{prefix}({string.Join(", ", indexArray.Select(index => componentNames[index]))})"
                : $"{prefix}({string.Join(", ", indexArray.Select(index => index.ToString()))})";
        }
    }
}
