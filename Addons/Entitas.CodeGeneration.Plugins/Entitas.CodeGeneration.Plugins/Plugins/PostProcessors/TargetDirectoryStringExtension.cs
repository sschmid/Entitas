using System;

namespace Entitas.CodeGeneration.Plugins {

    public static class TargetDirectoryStringExtension {

        public static string ToSafeDirectory(this string directory) {
            if(directory.EndsWith("/", StringComparison.Ordinal)) {
                directory = directory.Substring(0, directory.Length - 1);
            }

            if(!directory.EndsWith("/Generated", StringComparison.Ordinal)) {
                directory += "/Generated";
            }

            return directory;
        }
    }
}
