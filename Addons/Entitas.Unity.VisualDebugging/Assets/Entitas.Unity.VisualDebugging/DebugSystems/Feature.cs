namespace Entitas {

#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    public class Feature : Entitas.Unity.VisualDebugging.DebugSystems {

        public Feature(string name) {
            initialize(name);
        }

        public Feature() {
            initialize(toSpacedCamelCase(GetType().ToCompilableString().ShortTypeName()));
        }

        static string toSpacedCamelCase(string text) {
            var sb = new System.Text.StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for(int i = 1; i < text.Length; i++) {
                if(char.IsUpper(text[i]) && text[i - 1] != ' ') {
                    sb.Append(' ');
                }
                sb.Append(text[i]);
            }

            return sb.ToString();
        }
    }

#else

    public class Feature : Entitas.Systems {

        public Feature(string name) {
        }

        public Feature() {
        }
    }

#endif

}
