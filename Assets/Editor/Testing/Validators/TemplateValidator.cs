using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Test_TieuHoc.Validation
{
    public class TemplateValidator : IValidator
    {
        public string Name => "Kiểm tra Template và Activity";
        public string Description => "Kiểm tra các thiết lập template và activity trong project";
        
        private bool checkInactiveObjects = true;
        
        public bool CheckInactiveObjects
        {
            get => checkInactiveObjects;
            set => checkInactiveObjects = value;
        }

        public List<ValidationIssue> Validate()
        {
            var issues = new List<ValidationIssue>();
            
            // Find all prefabs in the project
            string[] prefabGuids = AssetDatabase.FindAssets("t:GameObject");
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".prefab")) continue;
                
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                // Check if it's an activity prefab
                if (prefab.name.Contains("Activity") || prefab.GetComponent("ActivityController") != null)
                {
                    // Check template usage
                    var template = prefab.GetComponent("TemplateController");
                    if (template != null)
                    {
                        // Check if template settings are modified
                        var serializedObject = new SerializedObject(template);
                        var modifiedProp = serializedObject.FindProperty("isModified");
                        if (modifiedProp != null && modifiedProp.boolValue)
                        {
                            var issue = new ValidationIssue();
                            issue.target = prefab;
                            issue.message = $"Template '{prefab.name}' đã bị chỉnh sửa không đúng quy định";
                            issue.severity = ValidationSeverity.Error;
                            issue.canAutoFix = true;
                            issues.Add(issue);
                        }
                    }
                    
                    // Check Canvas settings
                    var canvasComponents = prefab.GetComponentsInChildren<Canvas>(true);
                    foreach (var canvasComponent in canvasComponents)
                    {
                        // Check Canvas Scaler
                        var scalerComponent = canvasComponent.GetComponent<CanvasScaler>();
                        if (scalerComponent == null)
                        {
                            var issue = new ValidationIssue();
                            issue.target = canvasComponent.gameObject;
                            issue.message = $"Canvas trong '{prefab.name}' thiếu Canvas Scaler";
                            issue.severity = ValidationSeverity.Error;
                            issue.canAutoFix = true;
                            issues.Add(issue);
                        }
                        else
                        {
                            // Check Canvas Scaler settings
                            if (scalerComponent.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                            {
                                var issue = new ValidationIssue();
                                issue.target = scalerComponent;
                                issue.message = $"Canvas Scaler trong '{prefab.name}' không được set Scale With Screen Size";
                                issue.severity = ValidationSeverity.Error;
                                issue.canAutoFix = true;
                                issues.Add(issue);
                            }
                            
                            if (scalerComponent.referenceResolution != new Vector2(1920, 1080))
                            {
                                var issue = new ValidationIssue();
                                issue.target = scalerComponent;
                                issue.message = $"Canvas Scaler trong '{prefab.name}' không đúng reference resolution (1920x1080)";
                                issue.severity = ValidationSeverity.Error;
                                issue.canAutoFix = true;
                                issues.Add(issue);
                            }
                        }
                        
                        // Check render mode
                        if (canvasComponent.renderMode != RenderMode.ScreenSpaceOverlay)
                        {
                            var issue = new ValidationIssue();
                            issue.target = canvasComponent;
                            issue.message = $"Canvas trong '{prefab.name}' không set Screen Space - Overlay";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = true;
                            issues.Add(issue);
                        }
                    }
                }
            }
            
            return issues;
        }

        public void Fix(ValidationIssue issue)
        {
            if (issue.target is GameObject prefab && issue.message.Contains("template"))
            {
                var template = prefab.GetComponent("TemplateController");
                if (template != null)
                {
                    var serializedObject = new SerializedObject(template);
                    var modifiedProp = serializedObject.FindProperty("isModified");
                    if (modifiedProp != null)
                    {
                        modifiedProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            else if (issue.target is Canvas canvasComponent)
            {
                if (issue.message.Contains("Screen Space"))
                {
                    canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }
            else if (issue.target is CanvasScaler scalerComponent)
            {
                if (issue.message.Contains("Scale With Screen Size"))
                {
                    scalerComponent.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                }
                if (issue.message.Contains("reference resolution"))
                {
                    scalerComponent.referenceResolution = new Vector2(1920, 1080);
                }
            }
            else if (issue.target is GameObject targetObj && issue.message.Contains("Canvas Scaler"))
            {
                var existingCanvas = targetObj.GetComponent<Canvas>();
                if (existingCanvas != null)
                {
                    var scaler = targetObj.AddComponent<CanvasScaler>();
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    scaler.referenceResolution = new Vector2(1920, 1080);
                }
            }
        }

        public void FixAll()
        {
            var issues = Validate();
            foreach (var issue in issues)
            {
                Fix(issue);
            }
        }
    }
} 