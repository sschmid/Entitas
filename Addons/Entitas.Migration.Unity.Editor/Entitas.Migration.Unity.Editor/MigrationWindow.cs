using System;
using System.Linq;
using DesperateDevs.Unity.Editor;
using DesperateDevs.Utils;
using Entitas.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Migration.Unity.Editor
{
    public class MigrationWindow : EditorWindow
    {
        [MenuItem("Tools/Entitas/Migrate...", false, 1000)]
        public static void OpenMigrate()
        {
            var window = GetWindow<MigrationWindow>(true, "Entitas Migration - " + CheckForUpdates.GetLocalVersion());
            window.minSize = new Vector2(415f, 564);
            window.Show();
        }

        Texture2D _headerTexture;
        IMigration[] _migrations;
        bool[] _showMigration;
        Vector2 _scrollViewPosition;

        void OnEnable()
        {
            _headerTexture = EditorLayout.LoadTexture("l:EntitasHeader");
            _migrations = getMigrations();
            _showMigration = new bool[_migrations.Length];
            _showMigration[0] = true;
        }

        static IMigration[] getMigrations()
        {
            return AppDomain.CurrentDomain
                .GetInstancesOf<IMigration>()
                .OrderByDescending(instance => instance.GetType().FullName)
                .ToArray();
        }

        void OnGUI()
        {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                EditorLayout.DrawTexture(_headerTexture);

                var descriptionStyle = new GUIStyle(GUI.skin.label);
                descriptionStyle.wordWrap = true;
                for (int i = 0; i < _migrations.Length; i++)
                {
                    var migration = _migrations[i];
                    _showMigration[i] = EditorLayout.DrawSectionHeaderToggle(migration.version, _showMigration[i]);
                    if (_showMigration[i])
                    {
                        EditorLayout.BeginSectionContent();
                        {
                            EditorGUILayout.LabelField(migration.description, descriptionStyle);
                            if (GUILayout.Button("Apply migration " + migration.version))
                            {
                                migrate(migration, this);
                            }
                        }
                        EditorLayout.EndSectionContent();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        static void migrate(IMigration migration, MigrationWindow window)
        {
            var shouldMigrate = EditorUtility.DisplayDialog("Migrate",
                "You are about to migrate your source files. " +
                "Make sure that you have committed your current project or that you have a backup of your project before you proceed.",
                "I have a backup - Migrate",
                "Cancel"
            );

            if (shouldMigrate)
            {
                window.Close();
                EditorUtility.DisplayDialog("Migrate",
                    "Please select the folder, " + migration.workingDirectory + ".",
                    "I will select the requested folder"
                );

                var path = "Assets/";
                path = EditorUtility.OpenFolderPanel(migration.version + ": " + migration.workingDirectory, path, string.Empty);
                if (!string.IsNullOrEmpty(path))
                {
                    var changedFiles = migration.Migrate(path);
                    Debug.Log("Applying " + migration.version);
                    foreach (var file in changedFiles)
                    {
                        MigrationUtils.WriteFiles(changedFiles);
                        Debug.Log("Migrated " + file.fileName);
                    }
                }
                else
                {
                    throw new Exception("Could not complete migration! Selected path was invalid!");
                }
            }
        }
    }
}
