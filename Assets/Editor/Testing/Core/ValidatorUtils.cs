// Assets/Test_TieuHoc/Editor/Core/ValidatorUtils.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Các tiện ích dùng chung cho các validator
    /// </summary>
    public static class ValidatorUtils
    {
        /// <summary>
        /// Chọn một object trong hierarchy
        /// </summary>
        /// <param name="target">Object cần chọn</param>
        public static void SelectObject(Object target)
        {
            if (target != null)
            {
                Selection.activeObject = target;
                EditorGUIUtility.PingObject(target);
            }
        }
        
        /// <summary>
        /// Lấy đường dẫn đầy đủ của GameObject trong hierarchy
        /// </summary>
        /// <param name="obj">GameObject cần lấy đường dẫn</param>
        /// <returns>Đường dẫn đầy đủ</returns>
        public static string GetFullPath(GameObject obj)
        {
            if (obj == null)
                return "null";
                
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
        }
        
        /// <summary>
        /// Tìm tất cả objects theo type, hỗ trợ cả Unity phiên bản cũ và mới
        /// </summary>
        public static T[] FindAllObjectsOfType<T>() where T : Object
        {
            return Object.FindObjectsOfType<T>(true);
        }

        public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Object
        {
            return Object.FindObjectsOfType<T>(includeInactive);
        }

        public static T[] FindAssetsByType<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            
            return assets.ToArray();
        }
    }
}