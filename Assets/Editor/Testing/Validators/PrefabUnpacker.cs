// Assets/Test_TieuHoc/Editor/Validators/PrefabUnpacker.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra và unpack prefab nhân vật
    /// </summary>
    public class PrefabUnpacker : IValidator
    {
        public string Name => "Kiểm tra Prefab";
        public string Description => "Kiểm tra và unpack prefab nhân vật";
        
        // Thiết lập có thể tùy chỉnh
        public bool CheckCharactersOnly = true;
        public string[] CharacterKeywords = new string[] { "character", "char", "nhan_vat", "nhanvat", "nova", "player" };
        public bool CheckInactiveObjects = true;
        
        /// <summary>
        /// Kiểm tra tất cả prefab instances trong scene để tìm các prefab nhân vật chưa được unpack
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            
            // Tìm tất cả GameObjects trong scene
            GameObject[] allObjects = ValidatorUtils.FindObjectsOfType<GameObject>(true);
            
            foreach (var obj in allObjects)
            {
                // Bỏ qua objects không active nếu được cấu hình như vậy
                if (!CheckInactiveObjects && !obj.activeInHierarchy)
                    continue;
                
                // Kiểm tra xem đây có phải là prefab instance không
                if (IsPrefabInstance(obj))
                {
                    // Nếu chỉ kiểm tra nhân vật, thì kiểm tra xem object có phải nhân vật không
                    if (CheckCharactersOnly && !IsCharacterObject(obj))
                        continue;
                    
                    ValidationIssue issue = new ValidationIssue()
                    {
                        target = obj,
                        message = $"Prefab \"{obj.name}\" chưa được unpack",
                        severity = ValidationSeverity.Warning,
                        canAutoFix = true,
                        fixAction = () => UnpackPrefab(obj)
                    };
                    
                    issues.Add(issue);
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Unpack tất cả prefab nhân vật
        /// </summary>
        public void FixAll()
        {
            List<ValidationIssue> issues = Validate();
            foreach (var issue in issues)
            {
                if (issue.canAutoFix)
                    Fix(issue);
            }
        }
        
        /// <summary>
        /// Unpack một prefab cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.fixAction != null)
                issue.fixAction.Invoke();
        }
        
        /// <summary>
        /// Kiểm tra xem một GameObject có phải là prefab instance không
        /// </summary>
        private bool IsPrefabInstance(GameObject obj)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.IsPartOfPrefabInstance(obj) && !PrefabUtility.IsPartOfNonAssetPrefabInstance(obj);
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
            return prefabType == PrefabType.PrefabInstance;
#endif
        }
        
        /// <summary>
        /// Kiểm tra xem một GameObject có phải là nhân vật không
        /// </summary>
        private bool IsCharacterObject(GameObject obj)
        {
            string objNameLower = obj.name.ToLower();
            
            foreach (var keyword in CharacterKeywords)
            {
                if (objNameLower.Contains(keyword.ToLower()))
                    return true;
            }
            
            // Kiểm tra xem có component đặc trưng của nhân vật không
            // Ví dụ: Animator, SkinnedMeshRenderer, ...
            if (obj.GetComponent<Animator>() != null)
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Unpack một prefab
        /// </summary>
        private void UnpackPrefab(GameObject obj)
        {
#if UNITY_2018_3_OR_NEWER
            // Đối với Unity 2018.3 trở lên
            PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
#else
            // Đối với Unity phiên bản cũ hơn
            PrefabUtility.DisconnectPrefabInstance(obj);
#endif
            
            Debug.Log($"Đã unpack prefab: {obj.name}");
            
            // Đánh dấu scene là dirty để Unity biết cần lưu thay đổi
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }
    }
}