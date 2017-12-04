using System;
using System.Collections.Generic;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public static partial class EntityDrawer {

        static Dictionary<IContext, bool[]> _contextToUnfoldedComponents;
        public static Dictionary<IContext, bool[]> contextToUnfoldedComponents {
            get { if (_contextToUnfoldedComponents == null) { _contextToUnfoldedComponents = new Dictionary<IContext, bool[]>(); } return _contextToUnfoldedComponents; }
        }

        static Dictionary<IContext, string[]> _contextToComponentMemberSearch;
        public static Dictionary<IContext, string[]> contextToComponentMemberSearch {
            get { if (_contextToComponentMemberSearch == null) { _contextToComponentMemberSearch = new Dictionary<IContext, string[]>(); } return _contextToComponentMemberSearch; }
        }

        static Dictionary<IContext, GUIStyle[]> _contextToColoredBoxStyles;
        public static Dictionary<IContext, GUIStyle[]> contextToColoredBoxStyles {
            get { if (_contextToColoredBoxStyles == null) { _contextToColoredBoxStyles = new Dictionary<IContext, GUIStyle[]>(); } return _contextToColoredBoxStyles; }
        }

        public struct ComponentInfo {
            public int index;
            public string name;
            public Type type;
        }

        static Dictionary<IContext, ComponentInfo[]> _contextToComponentInfos;
        public static Dictionary<IContext, ComponentInfo[]> contextToComponentInfos {
            get { if (_contextToComponentInfos == null) { _contextToComponentInfos = new Dictionary<IContext, ComponentInfo[]>(); } return _contextToComponentInfos; }
        }

        static GUIStyle _foldoutStyle;
        public static GUIStyle foldoutStyle {
            get { if (_foldoutStyle == null) { _foldoutStyle = new GUIStyle(EditorStyles.foldout); _foldoutStyle.fontStyle = FontStyle.Bold; } return _foldoutStyle; }
        }

        static string _componentNameSearchString;
        public static string componentNameSearchString {
            get { if (_componentNameSearchString == null) { _componentNameSearchString = string.Empty; } return _componentNameSearchString; }
            set { _componentNameSearchString = value; }
        }

        public static readonly IDefaultInstanceCreator[] _defaultInstanceCreators;
        public static readonly ITypeDrawer[] _typeDrawers;
        public static readonly IComponentDrawer[] _componentDrawers;

        static EntityDrawer() {
            _defaultInstanceCreators = AppDomain.CurrentDomain.GetInstancesOf<IDefaultInstanceCreator>();
            _typeDrawers = AppDomain.CurrentDomain.GetInstancesOf<ITypeDrawer>();
            _componentDrawers = AppDomain.CurrentDomain.GetInstancesOf<IComponentDrawer>();
        }

        static bool[] getUnfoldedComponents(IContext context) {
            bool[] unfoldedComponents;
            if (!contextToUnfoldedComponents.TryGetValue(context, out unfoldedComponents)) {
                unfoldedComponents = new bool[context.totalComponents];
                for (int i = 0; i < unfoldedComponents.Length; i++) {
                    unfoldedComponents[i] = true;
                }
                contextToUnfoldedComponents.Add(context, unfoldedComponents);
            }

            return unfoldedComponents;
        }

        static string[] getComponentMemberSearch(IContext context) {
            string[] componentMemberSearch;
            if (!contextToComponentMemberSearch.TryGetValue(context, out componentMemberSearch)) {
                componentMemberSearch = new string[context.totalComponents];
                for (int i = 0; i < componentMemberSearch.Length; i++) {
                    componentMemberSearch[i] = string.Empty;
                }
                contextToComponentMemberSearch.Add(context, componentMemberSearch);
            }

            return componentMemberSearch;
        }

        static ComponentInfo[] getComponentInfos(IContext context) {
            ComponentInfo[] infos;
            if (!contextToComponentInfos.TryGetValue(context, out infos)) {
                var contextInfo = context.contextInfo;
                var infosList = new List<ComponentInfo>(contextInfo.componentTypes.Length);
                for (int i = 0; i < contextInfo.componentTypes.Length; i++) {
                    infosList.Add(new ComponentInfo {
                        index = i,
                        name = contextInfo.componentNames[i],
                        type = contextInfo.componentTypes[i]
                    });
                }
                infos = infosList.ToArray();
                contextToComponentInfos.Add(context, infos);
            }

            return infos;
        }

        static GUIStyle getColoredBoxStyle(IContext context, int index) {
            GUIStyle[] styles;
            if (!contextToColoredBoxStyles.TryGetValue(context, out styles)) {
                styles = new GUIStyle[context.totalComponents];
                for (int i = 0; i < styles.Length; i++) {
                    var hue = (float)i / (float)context.totalComponents;
                    var componentColor = Color.HSVToRGB(hue, 0.7f, 1f);
                    componentColor.a = 0.15f;
                    var style = new GUIStyle(GUI.skin.box);
                    style.normal.background = createTexture(2, 2, componentColor);
                    styles[i] = style;
                }
                contextToColoredBoxStyles.Add(context, styles);
            }

            return styles[index];
        }

        static Texture2D createTexture(int width, int height, Color color) {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i) {
                pixels[i] = color;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        static IComponentDrawer getComponentDrawer(Type type) {
            foreach (var drawer in _componentDrawers) {
                if (drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }

        static ITypeDrawer getTypeDrawer(Type type) {
            foreach (var drawer in _typeDrawers) {
                if (drawer.HandlesType(type)) {
                    return drawer;
                }
            }

            return null;
        }
    }
}
