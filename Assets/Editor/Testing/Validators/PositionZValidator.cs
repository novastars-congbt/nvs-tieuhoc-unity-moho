// Assets/Test_TieuHoc/Editor/Validators/PositionZValidator.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra và sửa Position Z bất thường của các objects
    /// </summary>
    public class PositionZValidator : IValidator
    {
        public string Name => "Kiểm tra Position Z";
        public string Description => "Kiểm tra và sửa Position Z bất thường của các objects (Z < -10)";
        
        // Thiết lập có thể tùy chỉnh
        public float MinZThreshold = -10f;
        public float DefaultZValue = 0f;
        public bool CheckOnlyVisibleObjects = false;
        
        /// <summary>
        /// Kiểm tra tất cả GameObject trong scene để tìm các object có Position Z bất thường
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            
            // Tìm tất cả GameObjects trong scene hiện tại
            GameObject[] allObjects = ValidatorUtils.FindAllObjectsOfType<GameObject>();
            
            foreach (GameObject obj in allObjects)
            {
                // Bỏ qua các objects ẩn nếu được cấu hình như vậy
                if (CheckOnlyVisibleObjects && !IsObjectVisible(obj))
                    continue;
                
                // Kiểm tra Position Z
                if (obj.transform.position.z < MinZThreshold)
                {
                    ValidationIssue issue = new ValidationIssue()
                    {
                        target = obj,
                        message = $"[{ValidatorUtils.GetFullPath(obj)}] có Position Z ({obj.transform.position.z:F2}) dưới ngưỡng {MinZThreshold}",
                        severity = ValidationSeverity.Warning,
                        canAutoFix = true,
                        fixAction = () => FixPositionZ(obj)
                    };
                    
                    issues.Add(issue);
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Sửa tất cả các vấn đề về Position Z
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
        /// Sửa một vấn đề cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.fixAction != null)
                issue.fixAction.Invoke();
        }
        
        /// <summary>
        /// Kiểm tra xem object có visible không
        /// </summary>
        private bool IsObjectVisible(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            return renderer != null && renderer.enabled && obj.activeInHierarchy;
        }
        
        /// <summary>
        /// Sửa Position Z về giá trị mặc định
        /// </summary>
        private void FixPositionZ(GameObject obj)
        {
            Undo.RecordObject(obj.transform, "Fix Position Z");
            Vector3 position = obj.transform.position;
            position.z = DefaultZValue;
            obj.transform.position = position;
            Debug.Log($"Đã sửa Position Z cho {obj.name} từ {position.z} thành {DefaultZValue}");
        }
    }
}