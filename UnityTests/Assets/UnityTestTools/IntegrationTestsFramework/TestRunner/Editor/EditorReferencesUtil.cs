using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest
{
    public static class EditorReferencesUtil
    {

        public static List<Object> FindScenesWhichContainAsset(string file)
        {
            string assetPath = GetAssetPathFromFileNameAndExtension (file);
            Object cur = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            return AllScenes.Where(a => ADependsOnB(a, cur)).ToList();
        }

        private static string CleanPathSeparators(string s)
        {
            const string forwardSlash = "/";
            const string backSlash = "\\";
            return s.Replace(backSlash, forwardSlash);
        }

        private static string GetRelativeAssetPathFromFullPath(string fullPath)
        {
            fullPath = CleanPathSeparators(fullPath);
            if (fullPath.Contains(Application.dataPath))
            {
                return fullPath.Replace(Application.dataPath, "Assets");
            }
            Debug.LogWarning("Path does not point to a location within Assets: " + fullPath);
            return null;
        }

        private static string GetAssetPathFromFileNameAndExtension(string assetName)
        {
            string[] assets = AssetDatabase.FindAssets (Path.GetFileNameWithoutExtension (assetName));
            string assetPath = null;
            
            foreach (string guid in assets) {
                string relativePath = AssetDatabase.GUIDToAssetPath (guid);
                
                if (Path.GetFileName (relativePath) == Path.GetFileName (assetName))
                    assetPath = relativePath;
            }
            
            return assetPath;
        }

        private static List<FileInfo> DirSearch(DirectoryInfo d, string searchFor)
        {
            List<FileInfo> founditems = d.GetFiles(searchFor).ToList();
            
            // Add (by recursing) subdirectory items.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
                founditems.AddRange(DirSearch(di, searchFor));
            
            return (founditems);
        }

        private static List<Object> AllScenes
        {
            get
            {
                // get every single one of the files in the Assets folder.
                List<FileInfo> files = DirSearch(new DirectoryInfo(Application.dataPath), "*.unity");
                
                // now make them all into Asset references.
                List<Object> assetRefs = new List<Object>();
                
                foreach (FileInfo fi in files)
                {
                    if (fi.Name.StartsWith("."))
                        continue;   // Unity ignores dotfiles.
                    assetRefs.Add(AssetDatabase.LoadMainAssetAtPath(GetRelativeAssetPathFromFullPath(fi.FullName)));
                }
                return assetRefs;
            }
        }

        private static bool ADependsOnB(Object obj, Object selectedObj)
        {
            if (selectedObj == null) return false;
            
            //optionally, exclude self.
            if (selectedObj == obj) return false;
            
            Object[] dependencies = EditorUtility.CollectDependencies(new Object[1] { obj });
            if (dependencies.Length < 2) return false;	 // if there's only one, it's us.
            
            foreach (Object dep in dependencies)
                if (dep == selectedObj)
                    return true;
            return false;
        }
    }
}