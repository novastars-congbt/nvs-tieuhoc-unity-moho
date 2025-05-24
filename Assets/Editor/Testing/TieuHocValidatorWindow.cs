// Assets/Test_TieuHoc/Editor/TieuHocValidatorWindow.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Cửa sổ chính của công cụ kiểm tra phần mềm tiểu học
    /// </summary>
    public class TieuHocValidatorWindow : EditorWindow
    {
        // UI state variables
        private Vector2 scrollPosition;
        private Vector2 batchScrollPosition;
        private List<IValidator> validators = new List<IValidator>();
        private Dictionary<IValidator, List<ValidationIssue>> validationResults = new Dictionary<IValidator, List<ValidationIssue>>();
        
        // Main tab selection
        private int selectedTab = 0;
        private string[] tabNames = { "Setup", "Check" };
        
        // Category tab selection
        private int selectedCategoryTab = 0;
        private string[] categoryTabNames = { "Cảnh và Object", "UI và Media", "Timeline và Data" };
        
        // Individual validator tab selection within each category
        private int selectedSceneObjectTab = 0;
        private int selectedUIMediaTab = 0;
        private int selectedTimelineDataTab = 0;
        
        // Arrays to hold validators by category for tab navigation
        private List<IValidator> sceneObjectValidators = new List<IValidator>();
        private List<IValidator> uiMediaValidators = new List<IValidator>();
        private List<IValidator> timelineDataValidators = new List<IValidator>();
        
        // Trạng thái UI
        private bool showPositionZSettings = false;
        private bool showTimelineCleanerSettings = false;
        private bool showFontSizeSettings = false;
        private bool showCameraSettings = false;
        private bool showPrefabUnpackerSettings = true;
        private bool showSortingGroupCheckerSettings = true;
        private bool showBorderVisibilityCheckerSettings = true;
        private bool showDataValidatorSettings = true;
        private bool[] validatorEnabled;
        private int resultTab = 0;
        
        // Validators
        private PositionZValidator positionZValidator;
        private TimelineCleaner timelineCleaner;
        private FontSizeValidator fontSizeValidator;
        private CameraValidator cameraValidator;
        private PrefabUnpacker prefabUnpacker;
        private SortingGroupChecker sortingGroupChecker;
        private BorderVisibilityChecker borderVisibilityChecker;
        private DataValidator dataValidator;
        private AudioValidator audioValidator;
        private TemplateValidator templateValidator;
        
        // Add threaded validation support
        private bool isValidating = false;
        private float validationProgress = 0f;
        private string validationStatusMessage = "";
        private System.Threading.Thread validationThread;
        
        // Add validator categories
        private enum ValidatorCategory
        {
            SceneObjects,
            UIAndMedia, 
            TimelineAndData
        }
        
        // Category mapping
        private Dictionary<IValidator, ValidatorCategory> validatorCategories = new Dictionary<IValidator, ValidatorCategory>();
        
        // Severity filters
        private bool showErrors = true;
        private bool showWarnings = true;
        private bool showInfo = true;
        
        // Category filters
        private bool showSceneObjects = true;
        private bool showUIAndMedia = true;
        private bool showTimelineAndData = true;
        
        // Add paging support to results display
        private int currentResultPage = 0;
        private const int resultsPerPage = 20;
        
        [MenuItem("Window/TieuHoc/Test")]
        public static void ShowWindow()
        {
            TieuHocValidatorWindow window = GetWindow<TieuHocValidatorWindow>("Check nhanh phần mềm tiểu học");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }
        
        private void OnEnable()
        {
            // Khởi tạo validators
            positionZValidator = new PositionZValidator();
            timelineCleaner = new TimelineCleaner();
            fontSizeValidator = new FontSizeValidator();
            cameraValidator = new CameraValidator();
            prefabUnpacker = new PrefabUnpacker();
            sortingGroupChecker = new SortingGroupChecker();
            borderVisibilityChecker = new BorderVisibilityChecker();
            dataValidator = new DataValidator();
            audioValidator = new AudioValidator();
            templateValidator = new TemplateValidator();

            validators.Clear();
            validators.Add(positionZValidator);
            validators.Add(timelineCleaner);
            validators.Add(fontSizeValidator);
            validators.Add(cameraValidator);
            validators.Add(prefabUnpacker);
            validators.Add(sortingGroupChecker);
            validators.Add(borderVisibilityChecker);
            validators.Add(dataValidator);
            validators.Add(audioValidator);
            validators.Add(templateValidator);
            
            // Khởi tạo trạng thái enabled cho các validator
            validatorEnabled = new bool[validators.Count];
            for (int i = 0; i < validatorEnabled.Length; i++)
            {
                validatorEnabled[i] = true;
            }

            // Mở sẵn phần cài đặt
            showPositionZSettings = true;
            showTimelineCleanerSettings = true;
            showFontSizeSettings = true;
            showCameraSettings = true;
            showPrefabUnpackerSettings = true;
            showSortingGroupCheckerSettings = true;
            showBorderVisibilityCheckerSettings = true;
            showDataValidatorSettings = true;

            // Load cached validation results if any
            LoadCachedResults();

            // Setup categories
            validatorCategories.Clear();
            validatorCategories[positionZValidator] = ValidatorCategory.SceneObjects;
            validatorCategories[prefabUnpacker] = ValidatorCategory.SceneObjects;
            validatorCategories[sortingGroupChecker] = ValidatorCategory.SceneObjects;
            validatorCategories[borderVisibilityChecker] = ValidatorCategory.SceneObjects;
            
            validatorCategories[fontSizeValidator] = ValidatorCategory.UIAndMedia;
            validatorCategories[cameraValidator] = ValidatorCategory.UIAndMedia;
            validatorCategories[audioValidator] = ValidatorCategory.UIAndMedia;
            
            validatorCategories[timelineCleaner] = ValidatorCategory.TimelineAndData;
            validatorCategories[dataValidator] = ValidatorCategory.TimelineAndData;
            validatorCategories[templateValidator] = ValidatorCategory.TimelineAndData;
            
            // Group validators by category for the tab interface
            sceneObjectValidators.Clear();
            uiMediaValidators.Clear();
            timelineDataValidators.Clear();
            
            foreach (var validator in validators)
            {
                if (validatorCategories.TryGetValue(validator, out ValidatorCategory category))
                {
                    switch (category)
                    {
                        case ValidatorCategory.SceneObjects:
                            sceneObjectValidators.Add(validator);
                            break;
                        case ValidatorCategory.UIAndMedia:
                            uiMediaValidators.Add(validator);
                            break;
                        case ValidatorCategory.TimelineAndData:
                            timelineDataValidators.Add(validator);
                            break;
                    }
                }
            }
        }
        
        private void OnDisable()
        {
            // Save cached validation results
            SaveCachedResults();
        }
        
        private void SaveCachedResults()
        {
            foreach (var validator in validators)
            {
                if (validationResults.ContainsKey(validator) && validationResults[validator] != null)
                {
                    int issueCount = validationResults[validator].Count;
                    EditorPrefs.SetInt($"TieuHocValidator_{validator.Name}_Count", issueCount);
                }
            }
        }
        
        private void LoadCachedResults()
        {
            foreach (var validator in validators)
            {
                if (EditorPrefs.HasKey($"TieuHocValidator_{validator.Name}_Count"))
                {
                    int issueCount = EditorPrefs.GetInt($"TieuHocValidator_{validator.Name}_Count");
                    if (issueCount > 0)
                    {
                        // Only load the count to show in the UI, actual validation will happen on demand
                        validationResults[validator] = new List<ValidationIssue>();
                    }
                }
            }
        }
        
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            
            // Header
            GUILayout.Space(10);
            GUILayout.Label("Công cụ kiểm tra nhanh phần mềm tiểu học", EditorStyles.boldLabel);
            GUILayout.Label("Kiểm tra và sửa lỗi phổ biến trong dự án Unity cho chương trình tiểu học", EditorStyles.miniLabel);
            GUILayout.Space(5);
            
            // Tabs
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            GUILayout.Space(10);
            
            // Main content based on selected tab
            switch (selectedTab)
            {
                case 0:
                    DrawSingleValidators();
                    break;
                case 1:
                    DrawBatchValidation();
                    break;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawSingleValidators()
        {
            GUILayout.BeginVertical();
            
            // Use scroll view to display validator settings
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Header
            EditorGUILayout.LabelField("Cấu hình kiểm tra", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Chọn danh mục và loại kiểm tra để cấu hình. Mỗi kiểm tra có thể được tùy chỉnh riêng.", MessageType.Info);
            EditorGUILayout.Space(10);
            
            // Category tabs
            selectedCategoryTab = GUILayout.Toolbar(selectedCategoryTab, categoryTabNames);
            EditorGUILayout.Space(5);
            
            // Draw the appropriate validators based on the selected category
            switch (selectedCategoryTab)
            {
                case 0: // Cảnh và Object
                    DrawCategoryValidators(sceneObjectValidators, ref selectedSceneObjectTab);
                    break;
                case 1: // UI và Media
                    DrawCategoryValidators(uiMediaValidators, ref selectedUIMediaTab);
                    break;
                case 2: // Timeline và Data
                    DrawCategoryValidators(timelineDataValidators, ref selectedTimelineDataTab);
                    break;
            }
            
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        
        private void DrawCategoryValidators(List<IValidator> categoryValidators, ref int selectedValidatorTab)
        {
            if (categoryValidators.Count == 0)
            {
                EditorGUILayout.HelpBox("Không có validator nào trong danh mục này.", MessageType.Info);
                return;
            }
            
            // Create tab names from validator names
            string[] validatorTabNames = categoryValidators.Select(v => v.Name).ToArray();
            
            // Draw tabs for validators in this category
            selectedValidatorTab = GUILayout.Toolbar(selectedValidatorTab, validatorTabNames);
            selectedValidatorTab = Mathf.Clamp(selectedValidatorTab, 0, categoryValidators.Count - 1);
            
            EditorGUILayout.Space(10);
            
            // Draw the selected validator settings
            IValidator selectedValidator = categoryValidators[selectedValidatorTab];
            
            // Draw validator in a box
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Header
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 14;
            EditorGUILayout.LabelField(selectedValidator.Name, titleStyle);
            
            // Description
            EditorGUILayout.LabelField(selectedValidator.Description, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(5);
            
            // Settings
            EditorGUILayout.LabelField("Thiết lập:", EditorStyles.boldLabel);
            
            // Draw the appropriate settings for the selected validator
            DrawValidatorSettings(selectedValidator);
            
            EditorGUILayout.Space(10);
            
            // Run button
            if (GUILayout.Button("Chạy kiểm tra này", GUILayout.Height(30)))
            {
                RunSingleValidator(selectedValidator);
            }
            
            // Show results if available
            if (validationResults.ContainsKey(selectedValidator) && validationResults[selectedValidator]?.Count > 0)
            {
                DrawValidatorResults(selectedValidator);
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawValidatorSettings(IValidator validator)
        {
            // Call the appropriate settings drawer based on validator type
            if (validator == positionZValidator)
                DrawPositionZValidatorSettings();
            else if (validator == timelineCleaner)
                DrawTimelineCleanerSettings();
            else if (validator == fontSizeValidator)
                DrawFontSizeValidatorSettings();
            else if (validator == cameraValidator)
                DrawCameraValidatorSettings();
            else if (validator == prefabUnpacker)
                DrawPrefabUnpackerSettings();
            else if (validator == sortingGroupChecker)
                DrawSortingGroupCheckerSettings();
            else if (validator == borderVisibilityChecker)
                DrawBorderVisibilityCheckerSettings();
            else if (validator == dataValidator)
                DrawDataValidatorSettings();
            else if (validator == audioValidator)
                DrawAudioValidatorSettings();
            else if (validator == templateValidator)
                DrawTemplateValidatorSettings();
        }
        
        private void DrawPositionZValidatorSettings()
        {
            positionZValidator.DefaultZValue = EditorGUILayout.FloatField("Giá trị Z mặc định:", positionZValidator.DefaultZValue);
            positionZValidator.MinZThreshold = EditorGUILayout.FloatField("Ngưỡng Z tối thiểu:", positionZValidator.MinZThreshold);
            positionZValidator.CheckOnlyVisibleObjects = EditorGUILayout.Toggle("Chỉ kiểm tra objects hiển thị:", positionZValidator.CheckOnlyVisibleObjects);
        }
        
        private void DrawTimelineCleanerSettings()
        {
            timelineCleaner.SearchInCurrentSceneOnly = EditorGUILayout.Toggle("Chỉ tìm trong scene hiện tại:", timelineCleaner.SearchInCurrentSceneOnly);
            timelineCleaner.IncludeSubEmptyTracks = EditorGUILayout.Toggle("Bao gồm các track con:", timelineCleaner.IncludeSubEmptyTracks);
        }
        
        private void DrawFontSizeValidatorSettings()
        {
            fontSizeValidator.MinTMProTextSize = EditorGUILayout.FloatField("Cỡ chữ tối thiểu TextMeshPro:", fontSizeValidator.MinTMProTextSize);
            fontSizeValidator.CheckInactiveObjects = EditorGUILayout.Toggle("Kiểm tra cả object không active:", fontSizeValidator.CheckInactiveObjects);
            fontSizeValidator.GroupByTextType = EditorGUILayout.Toggle("Nhóm kết quả theo loại text:", fontSizeValidator.GroupByTextType);
        }
        
        private void DrawCameraValidatorSettings()
        {
            cameraValidator.IgnoreDisabledGameObjects = EditorGUILayout.Toggle("Bỏ qua GameObject không active:", cameraValidator.IgnoreDisabledGameObjects);
            cameraValidator.CheckCinemachine = EditorGUILayout.Toggle("Kiểm tra camera Cinemachine:", cameraValidator.CheckCinemachine);
        }
        
        private void DrawPrefabUnpackerSettings()
        {
            prefabUnpacker.CheckCharactersOnly = EditorGUILayout.Toggle("Chỉ kiểm tra prefab nhân vật:", prefabUnpacker.CheckCharactersOnly);
            prefabUnpacker.CheckInactiveObjects = EditorGUILayout.Toggle("Kiểm tra cả object không active:", prefabUnpacker.CheckInactiveObjects);
        }
        
        private void DrawSortingGroupCheckerSettings()
        {
            sortingGroupChecker.CheckInactiveObjects = EditorGUILayout.Toggle("Kiểm tra object không hoạt động:", sortingGroupChecker.CheckInactiveObjects);
            sortingGroupChecker.CheckOnlyObjectsWithOrderInLayer = EditorGUILayout.Toggle("Chỉ kiểm tra object có Order in Layer:", sortingGroupChecker.CheckOnlyObjectsWithOrderInLayer);
        }
        
        private void DrawBorderVisibilityCheckerSettings()
        {
            borderVisibilityChecker.DeleteInsteadOfHide = EditorGUILayout.Toggle("Xóa thay vì ẩn Border:", borderVisibilityChecker.DeleteInsteadOfHide);
            
            EditorGUILayout.LabelField("Tên Border cần kiểm tra:");
            for (int i = 0; i < borderVisibilityChecker.BorderNames.Length; i++)
            {
                borderVisibilityChecker.BorderNames[i] = EditorGUILayout.TextField(borderVisibilityChecker.BorderNames[i]);
            }
        }
        
        private void DrawDataValidatorSettings()
        {
            dataValidator.AutoCleanData = EditorGUILayout.Toggle("Tự động làm sạch data:", dataValidator.AutoCleanData);
            dataValidator.DisallowedCharsPattern = EditorGUILayout.TextField("Mẫu ký tự không cho phép:", dataValidator.DisallowedCharsPattern);
        }
        
        private void DrawAudioValidatorSettings()
        {
            // Add settings for AudioValidator if needed
        }
        
        private void DrawTemplateValidatorSettings()
        {
            // Add settings for TemplateValidator if needed
        }
        
        private void DrawValidatorResults(IValidator validator)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Kết quả kiểm tra:", EditorStyles.boldLabel);
            
            var issues = validationResults[validator];
            
            // Group issues by severity
            var errorIssues = issues.Where(i => i.severity == ValidationSeverity.Error).ToList();
            var warningIssues = issues.Where(i => i.severity == ValidationSeverity.Warning).ToList();
            var infoIssues = issues.Where(i => i.severity == ValidationSeverity.Info).ToList();
            
            // Display counts
            EditorGUILayout.BeginHorizontal();
            if (errorIssues.Count > 0)
                GUILayout.Label(new GUIContent($" {errorIssues.Count} Lỗi", EditorGUIUtility.IconContent("console.erroricon").image));
            if (warningIssues.Count > 0)
                GUILayout.Label(new GUIContent($" {warningIssues.Count} Cảnh báo", EditorGUIUtility.IconContent("console.warnicon").image));
            if (infoIssues.Count > 0)
                GUILayout.Label(new GUIContent($" {infoIssues.Count} Thông tin", EditorGUIUtility.IconContent("console.infoicon").image));
                        GUILayout.EndHorizontal();
            
            // Fix all button
            if (issues.Any(i => i.canAutoFix))
            {
                if (GUILayout.Button("Sửa tất cả vấn đề", GUILayout.Height(25)))
                {
                    validator.FixAll();
                    validationResults.Remove(validator);
                    Repaint();
                }
            }
            
            // Display issues
            for (int i = 0; i < Mathf.Min(issues.Count, 10); i++)
            {
                var issue = issues[i];
                DrawSingleIssue(issue);
            }
            
            // Show more button if there are more than 10 issues
            if (issues.Count > 10)
            {
                if (GUILayout.Button($"Xem thêm {issues.Count - 10} vấn đề..."))
                {
                    // Switch to Check tab and focus on this validator
                    selectedTab = 1; // Check tab
                    
                    // Set filters to show only this validator's category
                    showSceneObjects = validatorCategories[validator] == ValidatorCategory.SceneObjects;
                    showUIAndMedia = validatorCategories[validator] == ValidatorCategory.UIAndMedia;
                    showTimelineAndData = validatorCategories[validator] == ValidatorCategory.TimelineAndData;
                }
            }
        }
        
        private void RunSingleValidator(IValidator validator)
        {
            validationStatusMessage = $"Đang chạy {validator.Name}...";
            EditorUtility.DisplayProgressBar("Đang kiểm tra", validationStatusMessage, 0.5f);
            
            try
            {
                var results = validator.Validate();
                validationResults[validator] = results;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Lỗi khi chạy validator {validator.Name}: {e.Message}\n{e.StackTrace}");
                EditorUtility.DisplayDialog("Lỗi", $"Xảy ra lỗi khi chạy validator {validator.Name}: {e.Message}", "OK");
            }
            
            EditorUtility.ClearProgressBar();
            Repaint();
        }
        
        private void DrawBatchValidation()
        {
            GUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("Kiểm tra nhiều mục cùng lúc", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Chọn các mục cần kiểm tra và nhấn nút 'Kiểm tra tất cả đã chọn'", MessageType.Info);
            
            // Show filter options
            DrawFilterOptions();
            
            // Check if we're validating and show progress
            if (isValidating)
            {
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 22f), 
                    validationProgress, validationStatusMessage);
                
                if (GUILayout.Button("Hủy kiểm tra", GUILayout.Height(30)))
                {
                    isValidating = false;
                    EditorApplication.update -= UpdateValidationProgress;
                    if (validationThread != null && validationThread.IsAlive)
                    {
                        validationThread.Abort();
                    }
                }
                
                GUILayout.Space(10);
            }
            else
            {
                // Display categories as tabs
                selectedCategoryTab = GUILayout.Toolbar(selectedCategoryTab, categoryTabNames);
                EditorGUILayout.Space(5);
                
                // Begin a box to contain the validators for the selected category
                GUILayout.BeginVertical(EditorStyles.helpBox);
                
                // Display validators for the selected category
                switch (selectedCategoryTab)
                {
                    case 0: // Cảnh và Object
                        DrawCategoryCheckList(ValidatorCategory.SceneObjects, "CẢNH VÀ OBJECT");
                        break;
                    case 1: // UI và Media
                        DrawCategoryCheckList(ValidatorCategory.UIAndMedia, "UI VÀ MEDIA");
                        break;
                    case 2: // Timeline và Data
                        DrawCategoryCheckList(ValidatorCategory.TimelineAndData, "TIMELINE VÀ DATA");
                        break;
                }
                
                GUILayout.EndVertical();
                
                GUILayout.Space(10);
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Chọn tất cả", GUILayout.Height(30)))
                {
                    for (int i = 0; i < validatorEnabled.Length; i++)
                        validatorEnabled[i] = true;
                }
                
                if (GUILayout.Button("Bỏ chọn tất cả", GUILayout.Height(30)))
                {
                    for (int i = 0; i < validatorEnabled.Length; i++)
                        validatorEnabled[i] = false;
                }
                GUILayout.EndHorizontal();
                
                if (GUILayout.Button("Kiểm tra tất cả đã chọn", GUILayout.Height(40)))
                {
                    RunSelectedValidators();
                }
                
                if (GUILayout.Button("Sửa tất cả vấn đề đã tìm thấy", GUILayout.Height(40)))
                {
                    FixAllIssues();
                }
            }
            
            // Display total issues count
            int totalIssues = validationResults.Values.Sum(list => list?.Count ?? 0);
            EditorGUILayout.LabelField($"Tổng số vấn đề: {totalIssues}");
            
            // Display results in a scrollable area
            batchScrollPosition = EditorGUILayout.BeginScrollView(batchScrollPosition, GUILayout.ExpandHeight(true));
            DrawAllResults();
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndVertical();
        }
        
        // Helper method to draw validators for a specific category
        private void DrawCategoryCheckList(ValidatorCategory category, string categoryTitle)
        {
            EditorGUILayout.LabelField(categoryTitle, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            // Find validators in this category
            for (int i = 0; i < validators.Count; i++)
            {
                if (validatorCategories.TryGetValue(validators[i], out ValidatorCategory validatorCategory) && validatorCategory == category)
                {
                    // Draw checkbox and name
                    GUILayout.BeginHorizontal();
                    validatorEnabled[i] = EditorGUILayout.ToggleLeft(validators[i].Name, validatorEnabled[i]);
                    GUILayout.EndHorizontal();
                    
                    // Show description in smaller text if enabled
                    if (validatorEnabled[i])
                    {
                        GUIStyle descriptionStyle = new GUIStyle(EditorStyles.miniLabel);
                        descriptionStyle.wordWrap = true;
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20); // Indent
                        GUILayout.Label(validators[i].Description, descriptionStyle);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
            }
        }
        
        // Keep the filter options in a tabbed interface too
        private void DrawFilterOptions()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Filter tabs
            string[] filterTabNames = new string[] { "Bộ lọc", "Phân loại" };
            resultTab = GUILayout.Toolbar(resultTab, filterTabNames);
            
            EditorGUILayout.Space(5);
            
            // Draw filter content based on selected tab
            switch (resultTab)
            {
                case 0: // Severity filters
                    DrawSeverityFilters();
                    break;
                case 1: // Category filters
                    DrawCategoryFilters();
                    break;
            }
            
            GUILayout.EndVertical();
        }
        
        private void DrawSeverityFilters()
        {
            EditorGUILayout.LabelField("Lọc theo độ nghiêm trọng:", EditorStyles.boldLabel);
            
            GUILayout.BeginVertical();
            showErrors = EditorGUILayout.ToggleLeft(new GUIContent(" Lỗi", EditorGUIUtility.IconContent("console.erroricon").image), showErrors);
            showWarnings = EditorGUILayout.ToggleLeft(new GUIContent(" Cảnh báo", EditorGUIUtility.IconContent("console.warnicon").image), showWarnings);
            showInfo = EditorGUILayout.ToggleLeft(new GUIContent(" Thông tin", EditorGUIUtility.IconContent("console.infoicon").image), showInfo);
            GUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
            
            // Quick buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Chọn tất cả"))
            {
                showErrors = showWarnings = showInfo = true;
            }
            
            if (GUILayout.Button("Bỏ chọn tất cả"))
            {
                showErrors = showWarnings = showInfo = false;
            }
            
            if (GUILayout.Button("Chỉ lỗi nghiêm trọng"))
            {
                showErrors = true;
                showWarnings = showInfo = false;
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawCategoryFilters()
        {
            EditorGUILayout.LabelField("Lọc theo danh mục:", EditorStyles.boldLabel);
            
            GUILayout.BeginVertical();
            showSceneObjects = EditorGUILayout.ToggleLeft("Cảnh và Object", showSceneObjects);
            showUIAndMedia = EditorGUILayout.ToggleLeft("UI và Media", showUIAndMedia);
            showTimelineAndData = EditorGUILayout.ToggleLeft("Timeline và Data", showTimelineAndData);
            GUILayout.EndVertical();
            
            EditorGUILayout.Space(5);
            
            // Quick buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Chọn tất cả"))
            {
                showSceneObjects = showUIAndMedia = showTimelineAndData = true;
            }
            
            if (GUILayout.Button("Bỏ chọn tất cả"))
            {
                showSceneObjects = showUIAndMedia = showTimelineAndData = false;
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawAllResults()
        {
            // Get filtered issues
            List<ValidationIssue> filteredIssues = new List<ValidationIssue>();
            
            foreach (var validator in validators)
            {
                if (!validationResults.ContainsKey(validator) || validationResults[validator] == null)
                    continue;
                
                // Check category filter
                bool categoryVisible = true;
                if (validatorCategories.ContainsKey(validator))
                {
                    switch (validatorCategories[validator])
                    {
                        case ValidatorCategory.SceneObjects:
                            categoryVisible = showSceneObjects;
                            break;
                        case ValidatorCategory.UIAndMedia:
                            categoryVisible = showUIAndMedia;
                            break;
                        case ValidatorCategory.TimelineAndData:
                            categoryVisible = showTimelineAndData;
                            break;
                    }
                }
                
                if (!categoryVisible)
                    continue;
                
                // Filter by severity
                foreach (var issue in validationResults[validator])
                {
                    bool shouldShow = false;
                    
                    switch (issue.severity)
                    {
                        case ValidationSeverity.Error:
                            shouldShow = showErrors;
                            break;
                        case ValidationSeverity.Warning:
                            shouldShow = showWarnings;
                            break;
                        case ValidationSeverity.Info:
                            shouldShow = showInfo;
                            break;
                    }
                    
                    if (shouldShow)
                        filteredIssues.Add(issue);
                }
            }
            
            int totalFilteredIssues = filteredIssues.Count;
            if (totalFilteredIssues == 0) return;
            
            GUILayout.Space(5);
            
            // Show summary of filtered results
            GUILayout.BeginHorizontal();
            
            int errorCount = filteredIssues.Count(i => i.severity == ValidationSeverity.Error);
            int warningCount = filteredIssues.Count(i => i.severity == ValidationSeverity.Warning);
            int infoCount = filteredIssues.Count(i => i.severity == ValidationSeverity.Info);
            
            if (showErrors)
                GUILayout.Label(new GUIContent($" {errorCount} Lỗi", EditorGUIUtility.IconContent("console.erroricon").image));
            
            if (showWarnings)
                GUILayout.Label(new GUIContent($" {warningCount} Cảnh báo", EditorGUIUtility.IconContent("console.warnicon").image));
            
            if (showInfo)
                GUILayout.Label(new GUIContent($" {infoCount} Thông tin", EditorGUIUtility.IconContent("console.infoicon").image));
            
            GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

            // Pagination
            int totalPages = Mathf.CeilToInt((float)totalFilteredIssues / resultsPerPage);
            currentResultPage = Mathf.Clamp(currentResultPage, 0, totalPages - 1);
            
            // Pagination controls
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("← Trang trước", GUILayout.Width(100)))
            {
                currentResultPage = Mathf.Max(0, currentResultPage - 1);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Trang {currentResultPage + 1}/{totalPages}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Trang sau →", GUILayout.Width(100)))
            {
                currentResultPage = Mathf.Min(totalPages - 1, currentResultPage + 1);
            }
            GUILayout.EndHorizontal();
            
            // Display current page
            int startIndex = currentResultPage * resultsPerPage;
            int endIndex = Mathf.Min(startIndex + resultsPerPage, totalFilteredIssues);
            
            GUILayout.BeginVertical(EditorStyles.helpBox);
            for (int i = startIndex; i < endIndex; i++)
            {
                DrawSingleIssue(filteredIssues[i]);
            }
            GUILayout.EndVertical();
        }
        
        private void DrawSingleIssue(ValidationIssue issue)
        {
            // Draw a more efficient single issue display
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                // Icon based on severity
                GUIContent icon = null;
                switch (issue.severity)
                {
                    case ValidationSeverity.Error:
                        icon = EditorGUIUtility.IconContent("console.erroricon");
                        break;
                    case ValidationSeverity.Warning:
                        icon = EditorGUIUtility.IconContent("console.warnicon");
                        break;
                    case ValidationSeverity.Info:
                        icon = EditorGUIUtility.IconContent("console.infoicon");
                        break;
                }
                
                if (icon != null)
                    GUILayout.Label(icon, GUILayout.Width(20));
                
            // Message - truncate if too long
            string shortMessage = issue.message;
            if (shortMessage.Length > 80)
                shortMessage = shortMessage.Substring(0, 77) + "...";
            
            GUILayout.Label(shortMessage, GUILayout.ExpandWidth(true));
                
                // Actions
                if (GUILayout.Button("Chọn", GUILayout.Width(60)))
                {
                    ValidatorUtils.SelectObject(issue.target);
                }
                
                if (issue.canAutoFix && GUILayout.Button("Sửa", GUILayout.Width(40)))
                {
                // Find the validator for this issue
                foreach (var validator in validators)
                {
                    if (validationResults.ContainsKey(validator) && 
                        validationResults[validator].Contains(issue))
                    {
                        validator.Fix(issue);
                        
                        // Remove the fixed issue from results
                        validationResults[validator].Remove(issue);
                        break;
                    }
                }
                
                Repaint();
            }
            
            GUILayout.EndHorizontal();
        }

        private void RunSelectedValidators()
        {
            if (isValidating)
                return;
            
            // Start validation in the background
            isValidating = true;
            validationProgress = 0f;
            validationStatusMessage = "Đang chuẩn bị kiểm tra...";
            
            EditorApplication.update += UpdateValidationProgress;
            
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(ValidateInBackground);
            validationThread = new System.Threading.Thread(threadStart);
            validationThread.Start();
        }
        
        private void ValidateInBackground()
        {
            try
            {
                Dictionary<IValidator, List<ValidationIssue>> results = 
                    new Dictionary<IValidator, List<ValidationIssue>>();
                
                int totalValidators = validators.Count;
                int validatorsDone = 0;
                
                for (int i = 0; i < validators.Count; i++)
                {
                    if (!validatorEnabled[i])
                    {
                        validatorsDone++;
                        validationProgress = (float)validatorsDone / totalValidators;
                        continue;
                    }
                    
                    IValidator validator = validators[i];
                    validationStatusMessage = $"Đang kiểm tra: {validator.Name}...";
                    
                    // Can't run the actual validation in a thread, so schedule it for the main thread
                    EditorApplication.delayCall += () => 
                    {
                        List<ValidationIssue> issues = validator.Validate();
                        lock(results)
                        {
                            results[validator] = issues;
                            validatorsDone++;
                            validationProgress = (float)validatorsDone / totalValidators;
                            
                            if (validatorsDone >= totalValidators)
                            {
                                // All done
                                validationResults = results;
                                isValidating = false;
                                EditorApplication.update -= UpdateValidationProgress;
                                Repaint();
                            }
                        }
                    };
                    
                    // Sleep to prevent locking up the UI
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error during validation: {e.Message}\n{e.StackTrace}");
                isValidating = false;
                EditorApplication.update -= UpdateValidationProgress;
            }
        }
        
        private void UpdateValidationProgress()
        {
            // Force a repaint to update progress bar
            Repaint();
        }
        
        private void FixAllIssues()
        {
            // Sửa tất cả vấn đề đã tìm thấy
            foreach (var validator in validators)
            {
                if (validationResults.ContainsKey(validator))
                {
                    validator.FixAll();
                    
                    // Cập nhật kết quả
                    List<ValidationIssue> newIssues = validator.Validate();
                    validationResults[validator] = newIssues;
                }
            }
        }
    }
}