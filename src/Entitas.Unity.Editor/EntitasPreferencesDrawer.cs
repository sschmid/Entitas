using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class EntitasPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => "Entitas";

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum AERCMode
        {
            Safe,
            FastAndUnsafe
        }

        Texture2D _headerTexture;
        ScriptingDefineSymbols _scriptingDefineSymbols;
        AERCMode _aercMode;

        public override void Initialize(Preferences preferences)
        {
            _headerTexture = EditorLayout.LoadTexture("l:EntitasHeader");

            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _aercMode = ScriptingDefineSymbols.BuildTargetGroups
                .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(ENTITAS_FAST_AND_UNSAFE))
                ? AERCMode.FastAndUnsafe
                : AERCMode.Safe;
        }

        public override void DrawHeader(Preferences preferences)
        {
            drawToolbar();
            drawHeader(preferences);
        }

        void drawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                if (GUILayout.Button("Chat", EditorStyles.toolbarButton))
                    EntitasFeedback.EntitasChat();

                if (GUILayout.Button("Wiki", EditorStyles.toolbarButton))
                    EntitasFeedback.EntitasWiki();

                if (GUILayout.Button("Donate", EditorStyles.toolbarButton))
                    EntitasFeedback.Donate();
            }
            EditorGUILayout.EndHorizontal();
        }

        void drawHeader(Preferences preferences) => EditorLayout.DrawTexture(_headerTexture);

        protected override void OnDrawContent(Preferences preferences)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Automatic Entity Reference Counting");
                var buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                if (_aercMode == AERCMode.Safe)
                    buttonStyle.normal = buttonStyle.active;

                if (GUILayout.Button("Safe", buttonStyle))
                {
                    _aercMode = AERCMode.Safe;
                    _scriptingDefineSymbols.RemoveForAll(ENTITAS_FAST_AND_UNSAFE);
                }

                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight);
                if (_aercMode == AERCMode.FastAndUnsafe)
                    buttonStyle.normal = buttonStyle.active;

                if (GUILayout.Button("Fast And Unsafe", buttonStyle))
                {
                    _aercMode = AERCMode.FastAndUnsafe;
                    _scriptingDefineSymbols.AddForAll(ENTITAS_FAST_AND_UNSAFE);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
