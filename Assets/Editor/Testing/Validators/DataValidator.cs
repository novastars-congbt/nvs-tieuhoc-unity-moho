// Assets/Test_TieuHoc/Editor/Validators/DataValidator.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra tên trong data không chứa ký tự đặc biệt
    /// </summary>
    public class DataValidator : IValidator
    {
        public string Name => "Kiểm tra Data";
        public string Description => "Kiểm tra tên trong data không chứa ký tự đặc biệt";
        
        // Thiết lập có thể tùy chỉnh
        public string[] DataFields = new string[] { "name", "id", "title", "key", "activity_name" };
        public string[] DataObjectNames = new string[] { "activity", "data", "content", "level" };
        public string DisallowedCharsPattern = @"[^a-zA-Z0-9_\-\s]";
        public bool AutoCleanData = false;
        
        /// <summary>
        /// Cấu trúc dữ liệu lưu trữ thông tin về field có vấn đề
        /// </summary>
        public class DataFieldInfo
        {
            public Object targetObject;
            public GameObject gameObject;
            public string fieldName;
            public string currentValue;
            public string cleanValue;
        }
        
        /// <summary>
        /// Kiểm tra tất cả MonoBehaviour có chứa data fields
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            var issues = new List<ValidationIssue>();
            
            // Use Object.FindObjectsOfType instead of ValidatorUtils.FindAllObjectsOfType
            var dataSources = Object.FindObjectsOfType<Component>(true)
                .Where(c => c.GetType().Name.Contains("Data") || 
                          c.name.Contains("Data") || 
                          c.GetType().Name.Contains("Manager"))
                .ToArray();
            
            foreach (var component in dataSources)
            {
                // Kiểm tra xem component có phải là data object không
                if (IsDataObject(component))
                {
                    // Tìm các data fields
                    List<DataFieldInfo> problemFields = FindProblemDataFields(component);
                    
                    // Tạo issues cho các fields có vấn đề
                    foreach (var fieldInfo in problemFields)
                    {
                        ValidationIssue issue = new ValidationIssue()
                        {
                            target = fieldInfo.targetObject,
                            message = $"Field \"{fieldInfo.fieldName}\" trong \"{component.name}\" chứa ký tự đặc biệt: \"{fieldInfo.currentValue}\"",
                            severity = ValidationSeverity.Error,
                            canAutoFix = true,
                            fixAction = () => CleanDataField(fieldInfo)
                        };
                        
                        issues.Add(issue);
                    }
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Sửa tất cả vấn đề về data
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
        /// Kiểm tra xem một component có phải là data object không
        /// </summary>
        private bool IsDataObject(Component component)
        {
            // Kiểm tra tên GameObject
            string objNameLower = component.gameObject.name.ToLower();
            
            foreach (var dataObjName in DataObjectNames)
            {
                if (objNameLower.Contains(dataObjName.ToLower()))
                    return true;
            }
            
            // Kiểm tra tên Component
            string componentName = component.GetType().Name.ToLower();
            
            foreach (var dataObjName in DataObjectNames)
            {
                if (componentName.Contains(dataObjName.ToLower()))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Tìm các data fields có vấn đề trong một component
        /// </summary>
        private List<DataFieldInfo> FindProblemDataFields(Component component)
        {
            List<DataFieldInfo> problemFields = new List<DataFieldInfo>();
            
            // Kiểm tra các SerializedProperty
            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                // Kiểm tra xem property này có phải là data field không
                if (IsDataField(iterator.name) && iterator.propertyType == SerializedPropertyType.String)
                {
                    string value = iterator.stringValue;
                    
                    // Kiểm tra xem có ký tự đặc biệt không
                    if (!string.IsNullOrEmpty(value) && HasSpecialCharacters(value))
                    {
                        DataFieldInfo fieldInfo = new DataFieldInfo()
                        {
                            targetObject = component,
                            gameObject = component.gameObject,
                            fieldName = iterator.name,
                            currentValue = value,
                            cleanValue = CleanString(value)
                        };
                        
                        problemFields.Add(fieldInfo);
                    }
                }
            }
            
            return problemFields;
        }
        
        /// <summary>
        /// Kiểm tra xem một field name có phải là data field không
        /// </summary>
        private bool IsDataField(string fieldName)
        {
            fieldName = fieldName.ToLower();
            
            foreach (var dataField in DataFields)
            {
                if (fieldName.Contains(dataField.ToLower()))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Kiểm tra xem một string có chứa ký tự đặc biệt không
        /// </summary>
        private bool HasSpecialCharacters(string input)
        {
            return Regex.IsMatch(input, DisallowedCharsPattern);
        }
        
        /// <summary>
        /// Làm sạch một string (loại bỏ ký tự đặc biệt)
        /// </summary>
        private string CleanString(string input)
        {
            // Thay thế ký tự đặc biệt bằng khoảng trắng
            string result = Regex.Replace(input, DisallowedCharsPattern, "_");
            
            // Loại bỏ các khoảng trắng đầu/cuối
            result = result.Trim();
            
            // Đảm bảo không có nhiều khoảng trắng liên tiếp
            result = Regex.Replace(result, @"\s+", " ");
            
            return result;
        }
        
        /// <summary>
        /// Làm sạch một data field
        /// </summary>
        private void CleanDataField(DataFieldInfo fieldInfo)
        {
            Component component = fieldInfo.targetObject as Component;
            if (component == null)
                return;
            
            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty property = serializedObject.FindProperty(fieldInfo.fieldName);
            
            if (property != null && property.propertyType == SerializedPropertyType.String)
            {
                Undo.RecordObject(component, "Clean Data Field");
                
                property.stringValue = fieldInfo.cleanValue;
                serializedObject.ApplyModifiedProperties();
                
                Debug.Log($"Đã làm sạch field {fieldInfo.fieldName} từ \"{fieldInfo.currentValue}\" thành \"{fieldInfo.cleanValue}\"");
                
                // Đánh dấu scene là dirty để Unity biết cần lưu thay đổi
                EditorUtility.SetDirty(component);
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }
    }
}