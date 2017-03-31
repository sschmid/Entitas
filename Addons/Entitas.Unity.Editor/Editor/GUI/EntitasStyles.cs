using UnityEngine;

namespace Entitas.Unity.Editor {

    public static class EntitasStyles {

        static GUIStyle _sectionHeader;
        public static GUIStyle sectionHeader {
            get {
                if(_sectionHeader == null) {
                    _sectionHeader = new GUIStyle("OL Title");
                }

                return _sectionHeader;
            }
        }

        static GUIStyle _sectionContent;
        public static GUIStyle sectionContent {
            get {
                if(_sectionContent == null) {
                    _sectionContent = new GUIStyle("OL Box");
                    _sectionContent.stretchHeight = false;
                }

                return _sectionContent;
            }
        }
    }
}
