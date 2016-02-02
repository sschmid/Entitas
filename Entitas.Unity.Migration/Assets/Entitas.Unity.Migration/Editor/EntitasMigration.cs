using System;
using System.Linq;
using System.Reflection;
using Entitas.Migration;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Migration {
    public class EntitasMigrationWindow : EditorWindow {

        [MenuItem("Entitas/Migrate...", false, 300)]
        public static void OpenMigrate() {
            EditorWindow.GetWindow<EntitasMigrationWindow>("Entitas Migration").Show();
        }

        IMigration[] _migrations;
        Vector2 _scrollViewPosition;

        void OnEnable() {
            _migrations = getMigrations();
        }

        static IMigration[] getMigrations() {
            return Assembly.GetAssembly(typeof(IMigration)).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IMigration)))
                .OrderByDescending(type => type.Name)
                .Select(type => (IMigration)Activator.CreateInstance(type))
                .ToArray();
        }

        void OnGUI() {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                var descriptionStyle = new GUIStyle(GUI.skin.label);
                descriptionStyle.wordWrap = true;
                foreach (var migration in _migrations) {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField(migration.version, EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(migration.description, descriptionStyle);
                        if (GUILayout.Button("Apply migration " + migration.version)) {
                            migrate(migration);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void migrate(IMigration migration) {
            var shouldMigrate = EditorUtility.DisplayDialog("Migrate",
                                    "You are about to migrate your source files. " +
                                    "Make sure that you have committed your current project or that you have a backup of your project before you proceed.",
                                    "I have a backup - Migrate",
                                    "Cancel"
                                );

            if (shouldMigrate) {
                EditorUtility.DisplayDialog("Migrate",
                    "Please select the folder, " + migration.workingDirectory + ".",
                    "I will select the requested folder"
                );

                var path = "Assets/";
                path = EditorUtility.OpenFolderPanel(migration.version + ": " + migration.workingDirectory, path, string.Empty);
                if (!string.IsNullOrEmpty(path)) {
                    var changedFiles = migration.Migrate(path);
                    Debug.Log("Applying " + migration.version);
                    foreach (var file in changedFiles) {
                        MigrationUtils.WriteFiles(changedFiles);
                        Debug.Log("Migrated " + file.fileName);
                    }
                } else {
                    throw new Exception("Could not complete migration! Selected path was invalid!");
                }
            }
        }
    }
}

