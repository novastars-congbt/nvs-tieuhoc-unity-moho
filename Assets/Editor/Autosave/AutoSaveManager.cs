using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AutoSaveManager : EditorWindow 
{
    // Core constants
    private const float MIN_AUTOSAVE_INTERVAL = 5f;      // minutes
    private const float MIN_SAVESTATE_INTERVAL = 15f;    // minutes
    private const string AUTO_DELETE_ENABLED_KEY = "AutoSave_AutoDeleteEnabled";
    private static bool autoDeleteEnabled = true;
    private const string MAX_SAVE_STATES_KEY = "AutoSave_MaxStates";
    private const int MIN_SAVE_STATES = 1;
    private const int DEFAULT_MAX_SAVE_STATES = 10;

    // Serializable data structures
    [System.Serializable]
    private class SaveStateInfo 
    {
        public string name;
        public DateTime timestamp;
        public string path;
        public bool isForced;
        public List<string> savedFiles = new List<string>();
        public string scenePath;
        public string sceneGuid;

        public string GetFormattedTime(){
            return timestamp.ToString("MM/dd/yyyy HH:mm:ss");
        }
    }

    // Editor state tracking
    private class EditorStates 
    {
        public bool isTimelineActive;
        public bool isSpriteEditorActive;
        public bool isAnimationRecording;
        public bool isPrefabMode;
        public bool isCompiling;
        public bool isPlayMode;
        public bool isVFXActive;
        public bool isUIBuilderActive;
        public bool isShaderGraphActive;
        public bool isProBuilderActive;

        private double lastUpdateTime;
        private const double UPDATE_INTERVAL = 0.5; // Check every 0.5 seconds

        public void Update()
        {
            // Only update states every UPDATE_INTERVAL seconds
            if (EditorApplication.timeSinceStartup - lastUpdateTime < UPDATE_INTERVAL)
                return;

            lastUpdateTime = EditorApplication.timeSinceStartup;
            
            isCompiling = EditorApplication.isCompiling;
            isPlayMode = EditorApplication.isPlaying;
            isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;

            // Reset flags
            isTimelineActive = false;
            isSpriteEditorActive = false;
            isAnimationRecording = false;
            isVFXActive = false;
            isUIBuilderActive = false;
            isShaderGraphActive = false;
            isProBuilderActive = false;

            // Only check focused window instead of all windows
            var focusedWindow = EditorWindow.focusedWindow;
            if (focusedWindow != null)
            {
                string windowType = focusedWindow.GetType().Name.ToLower();
                isTimelineActive = windowType.Contains("timeline");
                isSpriteEditorActive = windowType.Contains("spriteeditor");
                isAnimationRecording = windowType.Contains("animationwindow");
                isVFXActive = windowType.Contains("vfxeditor");
                isUIBuilderActive = windowType.Contains("uibuilder");
                isShaderGraphActive = windowType.Contains("shadergraph");
                isProBuilderActive = windowType.Contains("probuilder");
            }
        }
        
        public bool CanSaveCurrentState() 
        {
            return !isCompiling && !isPlayMode && !isPrefabMode &&
                !isTimelineActive && !isSpriteEditorActive && 
                !isAnimationRecording && !isVFXActive && 
                !isUIBuilderActive && !isShaderGraphActive && 
                !isProBuilderActive;
        }
    }

    // Static state
    private static double nextAutoSaveTime;
    private static double nextSaveStateTime;
    private static string saveStatePath;
    private static List<SaveStateInfo> saveStates = new List<SaveStateInfo>();
    private static EditorStates editorStates = new EditorStates();

    // Editor Prefs Keys
    private const string AUTO_SAVE_INTERVAL_KEY = "AutoSave_Interval";
    private const string SAVE_STATE_INTERVAL_KEY = "SaveState_Interval";
    private const string AUTO_SAVE_ENABLED_KEY = "AutoSave_Enabled";
    private const string SAVE_STATE_ENABLED_KEY = "SaveState_Enabled";

    // Settings
    private static float autoSaveInterval = MIN_AUTOSAVE_INTERVAL;
    private static float saveStateInterval = MIN_SAVESTATE_INTERVAL;
    private static bool autoSaveEnabled = true;
    private static bool saveStateEnabled = true;
    private static int maxSaveStates = DEFAULT_MAX_SAVE_STATES;

    // UI State
    private static Vector2 scrollPosition;
    private static bool isInitialized;
    private static GUIStyle headerStyle;
    private static GUIStyle cardStyle;
    private static GUIStyle statusDotStyle;
    private static Color activeColor = new Color(0.3f, 0.8f, 0.3f);
    private static Color inactiveColor = new Color(0.8f, 0.3f, 0.3f);
    private static Color processingColor = new Color(0.3f, 0.6f, 0.9f);
    private static string[] cachedScripts;
    private static double lastScriptCheck;
    private double lastRepaintTime;
    private const double REPAINT_INTERVAL = 0.25; // 4 times per second is smoother
    private bool needsRepaint;

    // Hot reload handling
    private static DateTime lastDomainReloadTime;
    private static bool isDomainReloading;
    private static readonly string domainReloadKey = "AutoSave_LastDomainReload";
    
    [MenuItem("Window/Auto Save Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<AutoSaveManager>("Auto Save");
        window.minSize = new Vector2(380, 520);
    }
    private void OnEnable()
    {
        // Delay the initialization to ensure EditorStyles are ready
        EditorApplication.delayCall += () =>
        {
            InitializeIfNeeded();
            EditorApplication.update += OnUpdate;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        };
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }
    private void InitializeStyles()
    {
        if (EditorStyles.boldLabel == null)
        {
            EditorApplication.delayCall += Repaint;
            return;
        }

        try
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(4, 4, 4, 4),
                    padding = new RectOffset(0, 0, 0, 0),
                    normal = { textColor = Color.white },
                    richText = true
                };
            }

            if (cardStyle == null)
            {
                cardStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(4, 4, 4, 4)
                };
            }

            if (statusDotStyle == null)
            {
                statusDotStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    fixedWidth = 20,
                    fixedHeight = 20
                };
            }
        }
        catch
        {
            EditorApplication.delayCall += Repaint;
        }
    }
    private static void InitializeIfNeeded()
    {
        if (isInitialized) return;

        // Load settings
        autoSaveInterval = EditorPrefs.GetFloat(AUTO_SAVE_INTERVAL_KEY, MIN_AUTOSAVE_INTERVAL);
        saveStateInterval = EditorPrefs.GetFloat(SAVE_STATE_INTERVAL_KEY, MIN_SAVESTATE_INTERVAL);
        autoSaveEnabled = EditorPrefs.GetBool(AUTO_SAVE_ENABLED_KEY, true);
        saveStateEnabled = EditorPrefs.GetBool(SAVE_STATE_ENABLED_KEY, true);
        autoDeleteEnabled = EditorPrefs.GetBool(AUTO_DELETE_ENABLED_KEY, true);
        maxSaveStates = EditorPrefs.GetInt(MAX_SAVE_STATES_KEY, DEFAULT_MAX_SAVE_STATES);

        // Setup save paths
        saveStatePath = Path.Combine(Application.dataPath, "..", "SaveStates");
        Directory.CreateDirectory(saveStatePath);

        // Initialize timers if needed
        if (nextAutoSaveTime <= 0)
        {
            ScheduleNextAutoSave();
        }
        if (nextSaveStateTime <= 0)
        {
            ScheduleNextSaveState();
        }

        // Load existing save states
        LoadSaveStates();

        isInitialized = true;
    }

    private static void OnBeforeAssemblyReload()
    {
        isDomainReloading = true;
        lastDomainReloadTime = DateTime.Now;
        EditorPrefs.SetString(domainReloadKey, lastDomainReloadTime.ToString("O"));
    }

    private static void OnAfterAssemblyReload()
    {
        isDomainReloading = false;
        string savedTimeStr = EditorPrefs.GetString(domainReloadKey, "");
        if (DateTime.TryParse(savedTimeStr, out DateTime savedTime))
        {
            lastDomainReloadTime = savedTime;
        }
    }

    private static void ScheduleNextAutoSave()
    {
        nextAutoSaveTime = EditorApplication.timeSinceStartup + (autoSaveInterval * 60);
    }

    private static void ScheduleNextSaveState()
    {
        nextSaveStateTime = EditorApplication.timeSinceStartup + (saveStateInterval * 60);
    }

    private void OnUpdate()
    {
        if (!isInitialized) return;

        editorStates.Update();

        // Only update if the window is visible
        if (!HasOpenInstances<AutoSaveManager>())
            return;

        double currentTime = EditorApplication.timeSinceStartup;

        // Check if it's time to repaint
        if (currentTime - lastRepaintTime >= REPAINT_INTERVAL)
        {
            needsRepaint = false;

            // Check auto save
            if (autoSaveEnabled && currentTime >= nextAutoSaveTime)
            {
                if (editorStates.CanSaveCurrentState())
                {
                    PerformAutoSave();
                }
                ScheduleNextAutoSave();
                needsRepaint = true;
            }

            // Check save state
            if (saveStateEnabled && currentTime >= nextSaveStateTime)
            {
                if (editorStates.CanSaveCurrentState())
                {
                    CreateSaveState(false);
                }
                ScheduleNextSaveState();
                needsRepaint = true;
            }

            // Always repaint if interval has passed (for timer updates)
            lastRepaintTime = currentTime;
            Repaint();
        }
    }
    private static void LoadSaveStates()
    {
        saveStates.Clear();
        
        if (Directory.Exists(saveStatePath))
        {
            var directories = Directory.GetDirectories(saveStatePath)
                .OrderByDescending(d => Directory.GetCreationTime(d));

            foreach (string dir in directories)
            {
                try
                {
                    var stateInfo = new SaveStateInfo
                    {
                        name = Path.GetFileName(dir),
                        timestamp = Directory.GetCreationTime(dir),
                        path = dir,
                        isForced = dir.Contains("_FORCE_")
                    };

                    // Load saved files list
                    string fileListPath = Path.Combine(dir, "FileList.txt");
                    if (File.Exists(fileListPath))
                    {
                        stateInfo.savedFiles = new List<string>(File.ReadAllLines(fileListPath));
                    }

                    saveStates.Add(stateInfo);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AutoSave] Error loading save state {dir}: {e.Message}");
                }
            }
        }
    }
    private static void PerformAutoSave()
    {
        try
        {
            if (!editorStates.CanSaveCurrentState()) return;

            var activeScene = EditorSceneManager.GetActiveScene();
            if (!activeScene.isDirty) return; // Skip if no changes

            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[AutoSave] Project saved at {DateTime.Now:HH:mm:ss}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AutoSave] Error during auto-save: {e.Message}");
        }
    }

    private static void CreateSaveState(bool isForced)
    {
        try
        {
            if (!editorStates.CanSaveCurrentState()) return;

            var activeScene = EditorSceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(activeScene.path))
            {
                Debug.LogWarning("[AutoSave] Cannot create save state: Scene has never been saved");
                return;
            }

            // Create a more organized name format
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string forcedTag = isForced ? "[FORCE]" : "[AUTO]";
            string sceneName = Path.GetFileNameWithoutExtension(activeScene.path); // Remove .unity extension
            string saveIndex = (saveStates.Count + 1).ToString("D2"); // 01, 02, etc.
            
            // Format: SceneName_[AUTO/FORCE]_SaveNumber_Date_Time
            string stateName = $"{sceneName}_{forcedTag}_{saveIndex}_{timestamp}";
            string statePath = Path.Combine(saveStatePath, stateName);

            // Ensure directory exists
            Directory.CreateDirectory(statePath);

            // Track files to save
            List<string> savedFiles = new List<string>();

            // Save current scene
            string sceneDestPath = Path.Combine(statePath, Path.GetFileName(activeScene.path));
            File.Copy(activeScene.path, sceneDestPath, true);
            savedFiles.Add(activeScene.path);

            // Save open scenes in additive mode
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene != activeScene && scene.path != string.Empty)
                {
                    string additiveScenePath = Path.Combine(statePath, Path.GetFileName(scene.path));
                    File.Copy(scene.path, additiveScenePath, true);
                    savedFiles.Add(scene.path);
                }
            }

            // Save modified prefabs in the scene
            var prefabs = FindModifiedPrefabsInScene();
            foreach (var prefab in prefabs)
            {
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                string prefabDestPath = Path.Combine(statePath, Path.GetFileName(prefabPath));
                File.Copy(prefabPath, prefabDestPath, true);
                savedFiles.Add(prefabPath);
            }

            // Save modified scripts
            var modifiedScripts = FindModifiedScripts();
            foreach (var script in modifiedScripts)
            {
                string scriptDestPath = Path.Combine(statePath, Path.GetFileName(script));
                File.Copy(script, scriptDestPath, true);
                savedFiles.Add(script);
            }

            // Save file list
            File.WriteAllLines(Path.Combine(statePath, "FileList.txt"), savedFiles);

            // Create save state info
            var stateInfo = new SaveStateInfo
            {
                name = stateName,
                timestamp = DateTime.Now,
                path = statePath,
                isForced = isForced,
                savedFiles = savedFiles,
                scenePath = activeScene.path,
                sceneGuid = AssetDatabase.AssetPathToGUID(activeScene.path)
            };

            // Add to list and maintain limit
            saveStates.Insert(0, stateInfo);
            while (saveStates.Count > maxSaveStates)
            {
                var oldState = saveStates[saveStates.Count - 1];
                if (Directory.Exists(oldState.path))
                {
                    if (autoDeleteEnabled)  // Add this condition
                    {
                        Directory.Delete(oldState.path, true);
                        saveStates.RemoveAt(saveStates.Count - 1);
                    }
                    else
                    {
                        // If auto-delete is disabled, don't create new save state
                        Debug.LogWarning("[AutoSave] Maximum save states reached. Enable auto-delete or manually remove old saves.");
                        return;
                    }
                }
            }

            Debug.Log($"[AutoSave] Created save state: {stateName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AutoSave] Error creating save state: {e.Message}");
        }
    }

    private static GameObject[] FindModifiedPrefabsInScene()
    {
        return GameObject.FindObjectsOfType<GameObject>()
            .Where(go => PrefabUtility.GetPrefabInstanceStatus(go) == PrefabInstanceStatus.Connected &&
                        PrefabUtility.HasPrefabInstanceAnyOverrides(go, false))
            .Select(go => PrefabUtility.GetCorrespondingObjectFromOriginalSource(go))
            .Where(prefab => prefab != null)
            .Distinct()
            .ToArray();
    }

    private static string[] FindModifiedScripts()
    {
        // Cache the result for 5 seconds
        if (cachedScripts != null && (EditorApplication.timeSinceStartup - lastScriptCheck) < 5)
        {
            return cachedScripts;
        }

        lastScriptCheck = EditorApplication.timeSinceStartup;
        cachedScripts = AssetDatabase.GetAllAssetPaths()
            .Where(path => path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && 
                        File.GetLastWriteTime(path) > DateTime.Now.AddMinutes(-30))
            .ToArray();
        
        return cachedScripts;
    }

    private static void RestoreSaveState(SaveStateInfo state)
    {
        try
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            // Restore each saved file
            foreach (var file in state.savedFiles)
            {
                string fileName = Path.GetFileName(file);
                string sourcePath = Path.Combine(state.path, fileName);
                
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, file, true);
                }
            }

            AssetDatabase.Refresh();

            // Reopen the scene
            if (!string.IsNullOrEmpty(state.scenePath))
            {
                EditorSceneManager.OpenScene(state.scenePath);
            }

            Debug.Log($"[AutoSave] Restored save state: {state.name}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[AutoSave] Error restoring save state: {e.Message}");
        }
    }
    private void OnGUI()
    {
        try
        {
            // Initialize if needed
            if (!isInitialized)
            {
                InitializeIfNeeded();
                return;
            }

            // Initialize or verify styles
            if (headerStyle == null || cardStyle == null || statusDotStyle == null)
            {
                InitializeStyles();
                return;
            }

            double currentTime = EditorApplication.timeSinceStartup;
            float autoSaveTimeLeft = autoSaveEnabled ? Mathf.Max(0, (float)(nextAutoSaveTime - currentTime)) : 0;
            float saveStateTimeLeft = saveStateEnabled ? Mathf.Max(0, (float)(nextSaveStateTime - currentTime)) : 0;

            EditorGUILayout.BeginVertical();
            {
                using (new EditorGUIUtility.IconSizeScope(Vector2.one * 16))
                {
                    DrawHeader();
                    DrawActiveFileSection();
                    DrawTimerSection(autoSaveTimeLeft, saveStateTimeLeft);
                    DrawSaveStatesList();
                    DrawStatusSection();
                    DrawHotReloadStatus();
                }
            }
            EditorGUILayout.EndVertical();
        }
        catch (ExitGUIException)
        {
            // This is expected when using GUIUtility.ExitGUI()
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in AutoSave Manager OnGUI: {e.Message}\n{e.StackTrace}");
            EditorApplication.delayCall += Repaint;
        }
    }
    private void DrawTimerSection(float autoSaveTimeLeft, float saveStateTimeLeft)
    {
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            // Auto Save Timer
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                autoSaveEnabled = EditorGUILayout.ToggleLeft("Auto Save:", autoSaveEnabled, GUILayout.Width(80));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(AUTO_SAVE_ENABLED_KEY, autoSaveEnabled);
                    needsRepaint = true;
                }

                using (new EditorGUI.DisabledScope(!autoSaveEnabled))
                {
                    EditorGUI.BeginChangeCheck();
                    autoSaveInterval = EditorGUILayout.FloatField(autoSaveInterval, GUILayout.Width(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        autoSaveInterval = Mathf.Max(MIN_AUTOSAVE_INTERVAL, autoSaveInterval);
                        EditorPrefs.SetFloat(AUTO_SAVE_INTERVAL_KEY, autoSaveInterval);
                        ScheduleNextAutoSave();
                        needsRepaint = true;
                    }
                    GUILayout.Label("min", GUILayout.Width(30));

                    if (autoSaveEnabled)
                    {
                        GUILayout.Label($"Next: {autoSaveTimeLeft:F0}s", GUILayout.Width(70));
                    }

                    if (GUILayout.Button("Save Now", GUILayout.Width(80)))
                    {
                        PerformAutoSave();
                        ScheduleNextAutoSave();
                        needsRepaint = true;
                    }
                }
            }

            // Save State Timer
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                saveStateEnabled = EditorGUILayout.ToggleLeft("Save State:", saveStateEnabled, GUILayout.Width(80));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(SAVE_STATE_ENABLED_KEY, saveStateEnabled);
                    needsRepaint = true;
                }

                using (new EditorGUI.DisabledScope(!saveStateEnabled))
                {
                    EditorGUI.BeginChangeCheck();
                    saveStateInterval = EditorGUILayout.FloatField(saveStateInterval, GUILayout.Width(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        saveStateInterval = Mathf.Max(MIN_SAVESTATE_INTERVAL, saveStateInterval);
                        EditorPrefs.SetFloat(SAVE_STATE_INTERVAL_KEY, saveStateInterval);
                        ScheduleNextSaveState();
                        needsRepaint = true;
                    }
                    GUILayout.Label("min", GUILayout.Width(30));

                    if (saveStateEnabled)
                    {
                        GUILayout.Label($"Next: {saveStateTimeLeft:F0}s", GUILayout.Width(70));
                    }

                    if (GUILayout.Button("Force", GUILayout.Width(80)))
                    {
                        CreateSaveState(true);
                        ScheduleNextSaveState();
                        needsRepaint = true;
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope()){
                EditorGUI.BeginChangeCheck();
                autoDeleteEnabled = EditorGUILayout.ToggleLeft("Auto Delete", autoDeleteEnabled, GUILayout.Width(120));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(AUTO_DELETE_ENABLED_KEY, autoDeleteEnabled);
                    needsRepaint = true;
                }

                GUILayout.Label("Max Saves:", GUILayout.Width(70));
                EditorGUI.BeginChangeCheck();
                maxSaveStates = EditorGUILayout.IntField(maxSaveStates, GUILayout.Width(50));
                if (EditorGUI.EndChangeCheck())
                {
                    maxSaveStates = Mathf.Max(MIN_SAVE_STATES, maxSaveStates);
                    EditorPrefs.SetInt(MAX_SAVE_STATES_KEY, maxSaveStates);
                    needsRepaint = true;
                }
            }
        }
    }
    private void DrawHeader()
    {
        // Use a toolbar style for the header container
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            // Add some spacing at the start
            GUILayout.Space(8);
            
            // Create a flexible space at the start
            GUILayout.FlexibleSpace();
            
            // Icon
            if (EditorGUIUtility.IconContent("d_Project") != null)
            {
                GUILayout.Label(EditorGUIUtility.IconContent("d_Project").image, 
                    GUILayout.Width(20), GUILayout.Height(20));
            }
            
            // Title
            GUILayout.Label("Auto Save Manager", headerStyle);
            
            // Add flexible space at the end
            GUILayout.FlexibleSpace();
            
            // Add some spacing at the end
            GUILayout.Space(8);
        }
        EditorGUILayout.EndHorizontal();
        
        // Add space after header
        EditorGUILayout.Space(4);
    }

    private void DrawActiveFileSection()
    {
        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            GUILayout.Label("Currently Tracking", EditorStyles.boldLabel);
            
            var activeScene = EditorSceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(activeScene.path))
            {
                GUILayout.Label($"▸ {activeScene.name}.unity", EditorStyles.miniLabel);
            }

            var modifiedScripts = FindModifiedScripts();
            if (modifiedScripts.Length > 0)
            {
                foreach (var script in modifiedScripts.Take(3))
                {
                    GUILayout.Label($"▸ {Path.GetFileName(script)}", EditorStyles.miniLabel);
                }
                if (modifiedScripts.Length > 3)
                {
                    GUILayout.Label($"   and {modifiedScripts.Length - 3} more...", EditorStyles.miniLabel);
                }
            }
        }
    }

    private void DrawSaveStatesList()
    {
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            // Header with Clear All button
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label($"Save States ({saveStates.Count}/{maxSaveStates})", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (saveStates.Count > 0)
                {
                    if (GUILayout.Button("Clear All", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        if (EditorUtility.DisplayDialog("Clear All Save States",
                            "Are you sure you want to delete all save states?",
                            "Delete All", "Cancel"))
                        {
                            DeleteAllSaveStates();
                            // Early return after clearing to avoid enumeration issues
                            return;
                        }
                    }
                }
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
            
            // Create a copy of the list for safe enumeration
            var statesToDraw = new List<SaveStateInfo>(saveStates);
            
            foreach (var state in statesToDraw)
            {
                // Check if the state still exists in the main list before drawing
                if (saveStates.Contains(state))
                {
                    DrawSaveStateCard(state);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawSaveStateCard(SaveStateInfo state)
    {
        if (!saveStates.Contains(state)) return;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            // Header
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(state.name, EditorStyles.boldLabel);
                if (state.isForced)
                {
                    GUILayout.Label("FORCE", EditorStyles.miniLabel);
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label(state.GetFormattedTime(), EditorStyles.miniLabel);
            }

            // Files
            foreach (var file in state.savedFiles.Take(3))
            {
                GUILayout.Label($"▸ {Path.GetFileName(file)}", EditorStyles.miniLabel);
            }
            if (state.savedFiles.Count > 3)
            {
                GUILayout.Label($"   and {state.savedFiles.Count - 3} more files...", EditorStyles.miniLabel);
            }

            // Buttons
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Load", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Load Save State",
                        "Loading a save state will revert your current changes. Continue?",
                        "Load", "Cancel"))
                    {
                        EditorApplication.delayCall += () => 
                        {
                            RestoreSaveState(state);
                            Repaint();
                        };
                    }
                }
                if (GUILayout.Button(new GUIContent("Delete", "Delete this save state"), GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete Save State",
                        "Are you sure you want to delete this save state?",
                        "Delete", "Cancel"))
                    {
                        EditorApplication.delayCall += () => 
                        {
                            DeleteSaveState(state);
                            Repaint();
                        };
                    }
                }
            }
        }
    }

    private static void DeleteSaveState(SaveStateInfo state)
    {
        try
        {
            if (Directory.Exists(state.path))
            {
                Directory.Delete(state.path, true);
            }
            saveStates.Remove(state);
            Debug.Log($"[AutoSave] Deleted save state: {state.name}");
            EditorApplication.delayCall += () => GetWindow<AutoSaveManager>().Repaint();
        }
        catch (Exception e)
        {
            Debug.LogError($"[AutoSave] Error deleting save state: {e.Message}");
        }
    }

    private static void DeleteAllSaveStates()
    {
        try
        {
            foreach (var state in saveStates.ToList())
            {
                if (Directory.Exists(state.path))
                {
                    Directory.Delete(state.path, true);
                }
            }
            saveStates.Clear();
            Debug.Log("[AutoSave] Cleared all save states");
            EditorApplication.delayCall += () => GetWindow<AutoSaveManager>().Repaint();
        }
        catch (Exception e)
        {
            Debug.LogError($"[AutoSave] Error clearing save states: {e.Message}");
        }
    }
    private void DrawStatusSection()
    {
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawStatusDot(autoSaveEnabled);
                GUILayout.Label("Auto Save", GUILayout.Width(80));
                
                GUILayout.Space(20);
                
                DrawStatusDot(saveStateEnabled);
                GUILayout.Label("Save State", GUILayout.Width(80));
            }
        }
    }

    private void DrawStatusDot(bool isEnabled)
    {
        Color originalColor = GUI.color;
        GUI.color = isEnabled ? activeColor : inactiveColor;
        GUILayout.Label("●", statusDotStyle);
        GUI.color = originalColor;
    }

    private void DrawHotReloadStatus()
    {
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            Color boxColor = isDomainReloading ? processingColor : 
                            (editorStates.isCompiling ? processingColor : activeColor);
            
            Color originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = boxColor;
            
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Hot Reload Status:");
                GUILayout.FlexibleSpace();
                
                string status = isDomainReloading ? "Reloading..." :
                            editorStates.isCompiling ? "Compiling..." : "Ready";
                            
                GUILayout.Label(status, EditorStyles.boldLabel);
            }
            
            GUI.backgroundColor = originalBgColor;

            if (lastDomainReloadTime != default)
            {
                GUILayout.Label($"Last reload: {lastDomainReloadTime:HH:mm:ss}", EditorStyles.miniLabel);
            }
        }
    }
}