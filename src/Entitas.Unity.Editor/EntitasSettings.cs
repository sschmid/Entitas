using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public sealed class EntitasSettings : ScriptableObject
    {
        public static EntitasSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    var guid = AssetDatabase.FindAssets($"l:{nameof(EntitasSettings)}").FirstOrDefault();
                    if (guid != null)
                    {
                        _instance = AssetDatabase.LoadAssetAtPath<EntitasSettings>(AssetDatabase.GUIDToAssetPath(guid));
                    }
                    else
                    {
                        var path = $"Assets/Editor/{nameof(EntitasSettings)}.asset";
                        AssetDatabase.CreateAsset(CreateInstance<EntitasSettings>(), path);
                        _instance = AssetDatabase.LoadAssetAtPath<EntitasSettings>(path);
                        AssetDatabase.SetLabels(_instance, new[] { nameof(EntitasSettings) });
                    }
                }

                return _instance;
            }
        }

        static EntitasSettings _instance;

        [Tooltip("Duration in milliseconds after which a system is considered to be slow. The system will be highlighted in the hierarchy view.")]
        public int SystemWarningThreshold = 5;
    }
}
