using System;
using Entitas.Migration;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Migration {
    public static class EntitasMigration {

        [MenuItem("Entitas/Migrate", false, 300)]
        public static void Migrate() {
            var allMigrations = new IMigration[] {
                new M0180(),
                new M0190(),
                new M0220(),
                new M0260()
            };

            var shouldMigrate = EditorUtility.DisplayDialog("Migrate",
                                    "You are about to migrate your source files. " +
                                    "Make sure that you have committed your current project or that you have a backup of your project before you proceed.",
                                    "I have a backup - Migrate",
                                    "Cancel"
                                );

            if (shouldMigrate) {

                EditorUtility.DisplayDialog("Migrate",
                    "There are " + allMigrations.Length + " migration steps. Please select the folder requested in the title bar of the 'Select Folder' panel.",
                    "I will select the requested folders"
                );

                var path = "Assets/";
                for (int i = 0; i < allMigrations.Length; i++) {
                    var migration = allMigrations[i];
                    var stepInfo = (i + 1) + "/" + allMigrations.Length;
                    path = EditorUtility.OpenFolderPanel(stepInfo + ": " + migration.version + ": " + migration.workingDirectory, path, string.Empty);
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
}

