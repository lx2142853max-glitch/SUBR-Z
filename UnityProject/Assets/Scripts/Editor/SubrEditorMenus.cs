#if UNITY_EDITOR
using SUBR.Data;
using UnityEditor;
using UnityEngine;

namespace SUBR.EditorTools
{
    public static class SubrEditorMenus
    {
        [MenuItem("SUBR/Create Default Match Config")]
        static void CreateMatchConfig()
        {
            var asset = ScriptableObject.CreateInstance<MatchConfig>();
            const string path = "Assets/ScriptableObjects/MatchConfig.asset";
            EnsureFolder("Assets/ScriptableObjects");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);
            Debug.Log($"[SUBR] Created {path}");
        }

        [MenuItem("SUBR/Create Sample Assault Rifle")]
        static void CreateAr()
        {
            var asset = ScriptableObject.CreateInstance<WeaponData>();
            asset.Id = 1;
            asset.DisplayName = "Assault Rifle";
            asset.Damage = 22f;
            asset.FireRate = 10f;
            asset.MagazineSize = 30;
            EnsureFolder("Assets/ScriptableObjects");
            const string path = "Assets/ScriptableObjects/Weapon_AR.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(asset);
        }

        static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                string cur = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    string next = cur + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(cur, parts[i]);
                    cur = next;
                }
            }
        }
    }
}
#endif
