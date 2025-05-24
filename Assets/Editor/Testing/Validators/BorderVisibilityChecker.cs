// Assets/Test_TieuHoc/Editor/Validators/BorderVisibilityChecker.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra BG_Border còn visible không
    /// </summary>
    public class BorderVisibilityChecker : IValidator
    {
        public string Name => "Kiểm tra BG_Border";
        public string Description => "Tìm BG_Border còn visible trong scene. Cần ẩn hoặc xóa trước khi build";
        
        // Thiết lập có thể tùy chỉnh
        public string[] BorderNames = new string[] { "BG_Border", "SafeZone", "Border", "Safe_Zone" };
        public bool DeleteInsteadOfHide { get; set; } = false;
        
        /// <summary>
        /// Kiểm tra các Border objects trong scene
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            
            // Tìm tất cả GameObject có tên chứa "BG_Border"
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
            
            foreach (var obj in allObjects)
            {
                if (obj.name.Contains("BG_Border") && obj.activeInHierarchy)
                {
                    var issue = new ValidationIssue();
                    issue.target = obj;
                    issue.message = $"BG_Border '{obj.name}' đang visible trong scene";
                    issue.severity = ValidationSeverity.Warning;
                    issue.canAutoFix = true;
                    issues.Add(issue);
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Ẩn hoặc xóa tất cả Border objects
        /// </summary>
        public void FixAll()
        {
            var issues = Validate();
            foreach (var issue in issues)
            {
                Fix(issue);
            }
        }
        
        /// <summary>
        /// Ẩn hoặc xóa một Border object cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.target is GameObject borderObj)
            {
                if (DeleteInsteadOfHide)
                {
                    Object.DestroyImmediate(borderObj);
                }
                else
                {
                    borderObj.SetActive(false);
                }
            }
            
            // Đánh dấu scene là dirty để Unity biết cần lưu thay đổi
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        
        /// <summary>
        /// Kiểm tra xem một GameObject có phải là Border object không
        /// </summary>
        private bool IsBorderObject(GameObject obj)
        {
            string objNameLower = obj.name.ToLower();
            
            foreach (var borderName in BorderNames)
            {
                if (objNameLower.Contains(borderName.ToLower()))
                    return true;
            }
            
            return false;
        }
    }
}