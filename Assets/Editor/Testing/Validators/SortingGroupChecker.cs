// Assets/Test_TieuHoc/Editor/Validators/SortingGroupChecker.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra objects cần có Sorting Group nhưng chưa có
    /// </summary>
    public class SortingGroupChecker : IValidator
    {
        public string Name => "Kiểm tra Sorting Group";
        public string Description => "Kiểm tra các object có chỉnh order in layer nhưng chưa thêm Sorting Group";
        
        // Settings
        public bool CheckInactiveObjects { get; set; } = true;
        public bool CheckOnlyObjectsWithOrderInLayer { get; set; } = true;
        
        /// <summary>
        /// Kiểm tra tất cả Renderers trong scene để tìm các objects cần có SortingGroup
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            var issues = new List<ValidationIssue>();
            
            // Find all renderers in the scene
            var renderers = Object.FindObjectsOfType<Renderer>(CheckInactiveObjects);
            
            foreach (var renderer in renderers)
            {
                if (renderer == null || renderer.gameObject == null) continue;
                
                // Skip if renderer has a SortingGroup component in its parent hierarchy
                if (renderer.gameObject.GetComponentInParent<SortingGroup>() != null)
                    continue;
                
                // Only check objects with modified order in layer
                if (CheckOnlyObjectsWithOrderInLayer && renderer.sortingOrder == 0)
                    continue;
                
                var issue = new ValidationIssue();
                issue.target = renderer.gameObject;
                issue.message = $"Object '{renderer.gameObject.name}' có Order in Layer ({renderer.sortingOrder}) nhưng không có Sorting Group";
                issue.severity = ValidationSeverity.Warning;
                issue.canAutoFix = true;
                issues.Add(issue);
            }
            
            return issues;
        }
        
        /// <summary>
        /// Thêm Sorting Group cho tất cả objects
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
        /// Thêm Sorting Group cho một object cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.target is GameObject go)
            {
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Save sorting values before adding group
                    int sortingOrder = renderer.sortingOrder;
                    string sortingLayerName = renderer.sortingLayerName;
                    
                    // Add sorting group
                    var sortingGroup = go.AddComponent<SortingGroup>();
                    sortingGroup.sortingOrder = sortingOrder;
                    sortingGroup.sortingLayerName = sortingLayerName;
                    
                    // Reset renderer's sorting values
                    renderer.sortingOrder = 0;
                }
            }
        }
    }
}