// Assets/Test_TieuHoc/Editor/Validators/CameraValidator.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator kiểm tra và phát hiện trường hợp nhiều camera active cùng lúc
    /// </summary>
    public class CameraValidator : IValidator
    {
        public string Name => "Kiểm tra Camera";
        public string Description => "Phát hiện trường hợp có nhiều camera active cùng lúc";
        
        // Thiết lập có thể tùy chỉnh
        public bool IgnoreDisabledGameObjects = true;
        public bool CheckCinemachine = true;
        public bool AutoCorrectCameraPriority = false;
        
        // Cấu trúc dữ liệu lưu trữ thông tin camera
        public class CameraInfo
        {
            public Camera camera;
            public GameObject gameObject;
            public string cameraType;
            public int priority;
            public bool isActiveAndEnabled;
        }
        
        /// <summary>
        /// Kiểm tra các camera trong scene để phát hiện trường hợp nhiều camera active
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();
            
            // Tìm tất cả camera
            List<CameraInfo> activeCameras = FindActiveCameras();
            
            // Kiểm tra số lượng camera active
            if (activeCameras.Count > 1)
            {
                // Tạo một issue chung về nhiều camera
                ValidationIssue multiCameraIssue = new ValidationIssue()
                {
                    target = null,
                    message = $"Có {activeCameras.Count} camera active cùng lúc trong scene",
                    severity = ValidationSeverity.Error,
                    canAutoFix = true,
                    fixAction = () => FixMultipleCameras(activeCameras)
                };
                
                issues.Add(multiCameraIssue);
                
                // Tạo issue cho từng camera
                foreach (var cameraInfo in activeCameras)
                {
                    ValidationIssue issue = new ValidationIssue()
                    {
                        target = cameraInfo.camera,
                        message = $"[{cameraInfo.cameraType}] {ValidatorUtils.GetFullPath(cameraInfo.gameObject)} đang active",
                        severity = ValidationSeverity.Warning,
                        canAutoFix = true,
                        fixAction = () => DisableCamera(cameraInfo)
                    };
                    
                    issues.Add(issue);
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Sửa tất cả các vấn đề về camera
        /// </summary>
        public void FixAll()
        {
            List<ValidationIssue> issues = Validate();
            
            if (issues.Count > 0 && issues[0].canAutoFix)
            {
                issues[0].fixAction?.Invoke();
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
        /// Tìm tất cả camera đang active trong scene
        /// </summary>
        private List<CameraInfo> FindActiveCameras()
        {
            List<CameraInfo> activeCameras = new List<CameraInfo>();
            
            // Tìm tất cả Camera trong scene
            Camera[] allCameras = ValidatorUtils.FindAllObjectsOfType<Camera>();
            
            foreach (var camera in allCameras)
            {
                // Bỏ qua camera trong GameObject không active
                if (IgnoreDisabledGameObjects && !camera.gameObject.activeInHierarchy)
                    continue;
                
                // Kiểm tra camera có enabled không
                if (!camera.enabled)
                    continue;
                
                string cameraType = "Standard";
                int priority = 0;
                
                // Phân loại camera
                if (camera.gameObject.name.Contains("Main") || camera.CompareTag("MainCamera"))
                {
                    cameraType = "Main Camera";
                    priority = 10;
                }
                else if (camera.gameObject.name.Contains("UI"))
                {
                    cameraType = "UI Camera";
                    priority = 5;
                }
                
                activeCameras.Add(new CameraInfo()
                {
                    camera = camera,
                    gameObject = camera.gameObject,
                    cameraType = cameraType,
                    priority = priority,
                    isActiveAndEnabled = camera.isActiveAndEnabled
                });
            }
            
            // Kiểm tra Cinemachine cameras nếu được yêu cầu
            if (CheckCinemachine)
            {
                FindCinemachineCameras(activeCameras);
            }
            
            return activeCameras;
        }
        
        /// <summary>
        /// Tìm tất cả camera Cinemachine trong scene
        /// </summary>
        private void FindCinemachineCameras(List<CameraInfo> activeCameras)
        {
            // Tìm Cinemachine Brain (điều khiển camera Cinemachine)
            var cinemachineBrainType = System.Type.GetType("Cinemachine.CinemachineBrain, Cinemachine");
            if (cinemachineBrainType != null)
            {
                var brains = Resources.FindObjectsOfTypeAll(cinemachineBrainType);
                foreach (var brain in brains)
                {
                    MonoBehaviour brainComponent = brain as MonoBehaviour;
                    if (brainComponent == null || !brainComponent.isActiveAndEnabled)
                        continue;
                    
                    // Lấy camera được điều khiển bởi Cinemachine Brain
                    var virtualCameraProperty = cinemachineBrainType.GetProperty("ActiveVirtualCamera");
                    if (virtualCameraProperty != null)
                    {
                        object virtualCamera = virtualCameraProperty.GetValue(brain);
                        if (virtualCamera != null)
                        {
                            var virtualCameraObject = new GameObject("Cinemachine Virtual Camera (Reference)");
                            
                            activeCameras.Add(new CameraInfo()
                            {
                                camera = brainComponent.GetComponent<Camera>(),
                                gameObject = brainComponent.gameObject,
                                cameraType = "Cinemachine",
                                priority = 20,
                                isActiveAndEnabled = true
                            });
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Sửa trường hợp có nhiều camera active
        /// </summary>
        private void FixMultipleCameras(List<CameraInfo> activeCameras)
        {
            if (activeCameras.Count <= 1)
                return;
            
            // Sắp xếp camera theo độ ưu tiên
            activeCameras.Sort((a, b) => b.priority.CompareTo(a.priority));
            
            // Giữ camera có độ ưu tiên cao nhất
            for (int i = 1; i < activeCameras.Count; i++)
            {
                DisableCamera(activeCameras[i]);
            }
            
            Debug.Log($"Đã giữ camera {activeCameras[0].gameObject.name} và tắt {activeCameras.Count - 1} camera khác");
        }
        
        /// <summary>
        /// Tắt một camera cụ thể
        /// </summary>
        private void DisableCamera(CameraInfo cameraInfo)
        {
            if (cameraInfo.camera != null)
            {
                Undo.RecordObject(cameraInfo.camera, "Disable Camera");
                cameraInfo.camera.enabled = false;
                EditorUtility.SetDirty(cameraInfo.camera);
                Debug.Log($"Đã tắt camera {cameraInfo.gameObject.name}");
            }
        }
    }
}