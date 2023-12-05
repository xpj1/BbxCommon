using System.IO;
using UnityEditor;
using UnityEngine;

namespace BbxCommon
{
    public static class ResourceApi
    {
#if UNITY_EDITOR
        public static TAsset LoadOrCreateAsset<TAsset>(string path) where TAsset : ScriptableObject
        {
            CreateDirectory(path);
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>(path);
            if (asset != null)
                return asset;
            else
            {
                if (path.EndsWith(".asset") == false)
                    path += ".asset";
                asset = ScriptableObject.CreateInstance<TAsset>();
                AssetDatabase.CreateAsset(asset, path);
                return asset;
            }
        }

        public static TAsset LoadOrCreateAssetInResources<TAsset>(string path) where TAsset : ScriptableObject
        {
            CreateDirectoryInResources(path);
            if (path.EndsWith(".asset") == false)
                path += ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>("Assets/Resources/" + path);
            if (asset != null)
                return asset;
            else
            {
                asset = ScriptableObject.CreateInstance<TAsset>();
                AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path);
                return asset;
            }
        }

        public static void CreateDirectory(string path)
        {
            if (path.EndsWith("/") == false)
            {
                int index = path.LastIndexOf("/");
                path = path.Remove(index + 1);
            }
            path = Application.dataPath + "/" + path.TryRemoveStart("Assets/");
            Directory.CreateDirectory(path);
        }

        public static void CreateDirectoryInResources(string path)
        {
            if (path.EndsWith("/") == false)
            {
                int index = path.LastIndexOf("/");
                path = path.Remove(index + 1);
            }
            path = Application.dataPath + "/Resources/" + path;
            Directory.CreateDirectory(path);
        }
#endif
    }
}
