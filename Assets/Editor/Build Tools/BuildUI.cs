using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using UnityEditor.SceneManagement;
using System;

public class BuildUI : EditorWindow
{
    private int currentClass = 1;
    private int currentLesson = 1;
    private int startClass = 1;
    private int endClass = 1;
    private int startLesson = 1;
    private int endLesson = 1;
    private bool uploadBuild = false;
    private int selectedTab = 0;
    private string[] tabNames = { "Single Build", "Multi Build" };
    private int buildCount = 0;
    private bool isVietnamese = false;
    private string errorMessage = "";
    private bool isWinSCPInstalled = false;
    private bool isTestBuild = false;
    private bool showVPSConfig = false; // For collapsible VPS info
    private bool isVpsConfigLoaded = false;
    private string lessonPrefabsPath = "Assets/Resources/Prefabs"; // Default path
    private string archivePrefabsPath = "Assets/5_Archive/Main"; // Default path
    private bool showLessonPath = true; // Control visibility of Lesson Path
    private bool showArchivePath = true; // Control visibility of Archive Path
    public Data lessonData; // Assign your Data Scriptable Object in the Inspector
    private string vpsHost;
    private int vpsPort = 22;
    private string vpsUsername;
    private string vpsPassword;
    private string winscpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.com";
    private string configFilePath => Path.Combine(Application.dataPath, "VPSConfig.json");
    private bool useIgnorePrefabs = true;
    private bool showAdvanced = false;
    private bool pendingUpdate = false;
    private float updateTimestamp = 0f;
    private float updateDelay = 0.75f;
    private string tempArchivePath;
    private Dictionary<string, string> translations = new Dictionary<string, string>
    {
        {"Move Prefabs to Archive", "Chuyển Prefab vào Lưu trữ"},
        {"Move Prefabs from Archive", "Chuyển Prefab từ Lưu trữ"},
        {"Single Build", "Build từng bài riêng"},
        {"Multi Build", "Build nhiều bài cùng lúc"},
        {"Build Specific", "Build bài lẻ"},
        {"Current Class", "Khối lớp"},
        {"Current Lesson", "Bài học"},
        {"Build Many", "Build nhiều bài cùng lúc"},
        {"Start Class", "Khối lớp đầu"},
        {"End Class", "Khối lớp cuối"},
        {"Start Lesson", "Bài đầu tiên"},
        {"End Lesson", "Bài cuối cùng"},
        {"Calculate Build Count", "Tính số lượng bài sẽ build"},
        {"Build All", "Build toàn bộ bài có sẵn"},
        {"Calculate All Builds", "Tổng số lượng bài sẽ build"},
        {"Upload Build", "Đưa bài lên VPS"},
        {"Build", "Build bài"},
        {"Success", "Thành công"},
        {"Error", "Lỗi"},
        {"Operation Complete", "Thao tác đã hoàn tất"},
        {"Prefab move operation completed.", "Prefab bài đã được đưa vào thư mục lưu trữ"},
        {"Confirm Build", "Xác nhận tiến hành quá trình build"},
        {"Are you sure you want to build", "Bạn có muốn bắt đầu quá trình build"},
        {"Are you sure you want to build and upload", "Bạn có muốn bắt đầu quá trình build và đưa bài lên VPS"},
        {"item(s)?", "mục?"},
        {"Yes", "Có"},
        {"No", "Không"},
        {"GameController data is not set to DataMain or DataSummer.", "Chưa đặt DataMain hoặc DataSummer trong canvas"},
        {"No prefabs found in the Resources folder.", "Không tìm thấy prefab trong thư mục Resources."},
        {"No prefabs found in the archive.", "Không tìm thấy prefab trong lưu trữ."},
        {"DataMain or DataSummer is not set in the Canvas of the current scene.", "DataMain hoặc DataSummer không được đặt trong Canvas của cảnh hiện tại."},
        {"Class", "Khối lớp"},
        {"Lesson", "Bài học"},
        {"Cannot build. Please ensure DataMain or DataSummer is set and archive prefabs exist.", "Không thể build. Vui lòng đảm bảo DataMain hoặc DataSummer được đặt và có prefab trong thư mục lưu trữ."},
        {"WinSCP is not installed. Please install WinSCP to use the upload feature.", "WinSCP chưa được cài đặt. Vui lòng cài đặt WinSCP để sử dụng tính năng tải lên."},
        {"Test Build", "Build bản test"},
        {"Confirm Test Build", "Xác nhận build bản test"},
        {"Are you sure you want to build and upload the test build", "Bạn có muốn bắt đầu quá trình build bản test lên VPS"},
        {"This will rename the build to lop-1-99", "Build sẽ được đổi tên thành lop-1-99"},
        {"Upload is required for test builds", "Bản build test phải được upload lên VPS"}
    };

    [MenuItem("Tools/Build Utility")]
    public static void ShowWindow()
    {
        BuildUI window = GetWindow<BuildUI>("Build Utility");
        window.LoadVPSConfiguration(); // Load saved configuration on window open
    }

    private void OnGUI()
    {
        bool hasPrefabs = HasMatchingLessonFolders(lessonPrefabsPath);
        bool hasArchivePrefabs = HasMatchingLessonFolders(useIgnorePrefabs ? "Assets/.Ignore/Resources/Prefabs" : archivePrefabsPath);
        bool isDataMainSet = IsDataMainSet();
        
        bool notEmpty = Directory.Exists(lessonPrefabsPath) && Directory.GetDirectories(lessonPrefabsPath).Length > 0;
        DrawLanguageToggle();

        GUILayout.Space(10);

        // VPS Configuration
        if (GUILayout.Button("VPS Info"))
        {
            showVPSConfig = !showVPSConfig;
        }
        if (showVPSConfig)
        {
            DrawVPSConfiguration();
        }

        GUILayout.Space(10);

        DrawPathsAndDataObject();
        
        // Data Scriptable Object field
        lessonData = EditorGUILayout.ObjectField("Data Scriptable Object", lessonData, typeof(Data), false) as Data;

        // Preview and Revert Buttons (Single Build Tab)
        if (selectedTab == 0)
        {
            buildCount = 1;
            if (lessonData != null)
            {
                GUILayout.BeginHorizontal();
                GUI.enabled = hasArchivePrefabs;
                if (GUILayout.Button("Preview"))
                {
                    PreviewAssets(lessonPrefabsPath, archivePrefabsPath, currentClass, currentLesson, lessonData);
                }
                GUI.enabled = DoesLessonFolderExist(currentClass, currentLesson);
                if (GUILayout.Button("Update"))
                {
                    if (EditorUtility.DisplayDialog("Confirm Update", "Are you sure you want to update resources?", "Yes", "No"))
                    {
                        UpdateResources();
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal(); // Ensure this matches BeginHorizontal

                // Collapsible section for Revert and Cleanup
                showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Revert and Cleanup Options", true);
                GUILayout.BeginHorizontal(); // Start horizontal layout

                if (showAdvanced)
                {
                    GUI.enabled = HasBackup(currentClass, currentLesson);
                    if (GUILayout.Button("Revert"))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Revert", "Are you sure you want to revert to a backup?", "Yes", "No"))
                        {
                            RevertToBackup();
                        }
                    }
                    GUI.enabled = true;

                    GUI.enabled = hasPrefabs;
                    if (GUILayout.Button("Cleanup"))
                    {
                        if (EditorUtility.DisplayDialog("Confirm Cleanup", "Are you sure you want to clean up resources?", "Yes", "No"))
                        {
                            CleanupResources();
                        }
                    }
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal(); // End horizontal layout, outside the conditional
            }
            else
            {
                EditorGUILayout.HelpBox("Please assign a Data Scriptable Object.", MessageType.Warning);
            }
        }

        // Top section: Move Prefab buttons
        EditorGUILayout.BeginHorizontal();
        bool preview = hasArchivePrefabs && notEmpty;
        // Move Prefabs to Archive button
        GUI.enabled = !preview && hasPrefabs && isDataMainSet;
        if (GUILayout.Button(GetTranslatedText("Move Prefabs to Archive")))
        {
            string targetArchivePath = useIgnorePrefabs ? "Assets/.Ignore/Resources/Prefabs" : archivePrefabsPath;
            if (AssetManager.MovePrefabsToArchive(lessonPrefabsPath, targetArchivePath))
            {
                EditorUtility.DisplayDialog(GetTranslatedText("Operation Complete"), GetTranslatedText("Prefab move operation completed."), "OK");
            }
            else
            {
                EditorUtility.DisplayDialog(GetTranslatedText("Error"), GetTranslatedText("Failed to move prefabs."), "OK");
            }
        }

        // Move Prefabs from Archive button
        GUI.enabled = !notEmpty && hasArchivePrefabs;
        if (GUILayout.Button(GetTranslatedText("Move Prefabs from Archive")))
        {
            string sourceArchivePath = useIgnorePrefabs ? "Assets/.Ignore/Resources/Prefabs" : archivePrefabsPath;
            if (AssetManager.MovePrefabsFromArchive(sourceArchivePath, lessonPrefabsPath))
            {
                EditorUtility.DisplayDialog(GetTranslatedText("Operation Complete"), GetTranslatedText("Prefab move operation completed."), "OK");
            }
            else
            {
                EditorUtility.DisplayDialog(GetTranslatedText("Error"), GetTranslatedText("Failed to move prefabs."), "OK");
            }
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Tabs
        selectedTab = GUILayout.Toolbar(selectedTab, new string[] { GetTranslatedText("Single Build"), GetTranslatedText("Multi Build") });

        if (selectedTab == 0)
        {
            DrawSingleBuildTab(isDataMainSet, hasArchivePrefabs);
        }
        else
        {
            DrawMultiBuildTab();
        }

        GUILayout.Space(10);

        // Bottom section: Build button and options
        EditorGUILayout.BeginHorizontal();
        
        // Only allow toggling upload if not in test build mode
        if (!isTestBuild)
        {
            bool newUploadBuild = EditorGUILayout.Toggle(GetTranslatedText("Upload Build"), uploadBuild);
            if (newUploadBuild != uploadBuild)
            {
                uploadBuild = newUploadBuild;
                if (uploadBuild)
                {
                    isWinSCPInstalled = CheckWinSCPInstallation();
                    if (!isVpsConfigLoaded || !isWinSCPInstalled)
                    {
                        EditorGUILayout.HelpBox("Upload and Test Build are disabled. Ensure VPS info is complete and WinSCP is installed.", MessageType.Warning);
                        uploadBuild = false;
                    }
                }
            }
        }
        else
        {
            // Show disabled upload toggle when in test build mode
            GUI.enabled = false;
            EditorGUILayout.Toggle(GetTranslatedText("Upload Build"), true);
            GUI.enabled = true;
        }

        GUI.enabled = isDataMainSet && hasArchivePrefabs && (!uploadBuild || (uploadBuild && isWinSCPInstalled));
        if (GUILayout.Button(GetTranslatedText("Build")))
        {
            ConfirmAndPerformBuild();
        }
        GUI.enabled = true;
        EditorGUILayout.TextField(buildCount.ToString(), GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();

        // Error message section
        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }
        // Check if the delay has passed and update if necessary
        if (pendingUpdate && (EditorApplication.timeSinceStartup - updateTimestamp >= updateDelay))
        {
            UpdateGameController(currentClass - 1, currentLesson - 1);
            pendingUpdate = false;
        }
    }
    // Helper method to check for backups
    private bool HasBackup(int classNum, int lessonNum)
    {
        // Check if a relevant backup directory exists
        string backupDir = Path.Combine("Assets/.Ignore/Backups");
        if (!Directory.Exists(backupDir))
        {
            return false;
        }
        return Directory.GetDirectories(backupDir, $"L{classNum}_{lessonNum}_*").Any();
    }
    private bool DoesLessonFolderExist(int classNum, int lessonNum)
    {
        // Construct the directory path based on your folder structure
        string lessonFolder = Path.Combine(lessonPrefabsPath, $"L{classNum}", $"L{classNum}_{lessonNum}");
        return Directory.Exists(lessonFolder);
    }
    private void UpdateGameController(int classNum, int lessonNum)
    {
        string firstScenePath = EditorBuildSettings.scenes.First().path;
        EditorSceneManager.OpenScene(firstScenePath, OpenSceneMode.Single);
        GameController controller = GameObject.FindObjectOfType<GameController>();
        if (controller != null)
        {
            controller.currentClass = classNum;
            controller.currentLesson = lessonNum;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Debug.Log($"Updated GameController: Class {classNum + 1}, Lesson {lessonNum + 1} (input was zero-based)");
        }
        else
        {
            Debug.LogError("GameController component not found in the opening scene.");
        }
    }
    private void DrawPathsAndDataObject()
    {
        // Initialize original path if not set
        if (string.IsNullOrEmpty(tempArchivePath))
        {
            tempArchivePath = archivePrefabsPath;
        }

        // Lesson Prefabs Path
        EditorGUILayout.BeginHorizontal();
        showLessonPath = EditorGUILayout.Foldout(showLessonPath, "Lesson Prefabs Path", true);
        GUI.enabled = false;
        lessonPrefabsPath = EditorGUILayout.TextField(lessonPrefabsPath);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        if (showLessonPath)
        {
            lessonPrefabsPath = DrawDragAndDropArea(lessonPrefabsPath);
        }

        // Archive Prefabs Path
        EditorGUILayout.BeginHorizontal();
        showArchivePath = EditorGUILayout.Foldout(showArchivePath, "Archive Prefabs Path", true);
        EditorGUILayout.EndHorizontal();

        // Toggle for using .Ignore path
        EditorGUILayout.BeginHorizontal();
        useIgnorePrefabs = EditorGUILayout.ToggleLeft("Use .Ignore Prefabs", useIgnorePrefabs, GUILayout.Width(130));

        GUI.enabled = false;
        if (useIgnorePrefabs)
        {
            EditorGUILayout.TextField("Assets/.Ignore/Resources/Prefabs");
        }
        else
        {
            EditorGUILayout.TextField(archivePrefabsPath);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        if (!useIgnorePrefabs && showArchivePath)
        {
            archivePrefabsPath = DrawDragAndDropArea(archivePrefabsPath);
        }
    }
    private string DrawDragAndDropArea(string path)
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

        // Customize the drag-and-drop box
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.normal.textColor = Color.white;
        boxStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.4f, 0.6f, 0.5f));

        GUI.Box(dropArea, "Drag and Drop Folder Here", boxStyle);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (string draggedPath in DragAndDrop.paths)
                    {
                        if (Directory.Exists(draggedPath))
                        {
                            path = draggedPath;
                            Debug.Log("Dropped Folder: " + path);
                        }
                        else
                        {
                            Debug.LogWarning("Dropped item is not a folder: " + draggedPath);
                        }
                    }
                }
                Event.current.Use();
                break;
        }
        return path;
    }

    // Helper method to create a texture for the background
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    private void DrawVPSConfiguration()
    {
        EditorGUILayout.LabelField("VPS Configuration", EditorStyles.boldLabel);

        vpsHost = EditorGUILayout.TextField("VPS Host", vpsHost);
        vpsPort = EditorGUILayout.IntField("VPS Port", vpsPort);
        vpsUsername = EditorGUILayout.TextField("VPS Username", vpsUsername);
        vpsPassword = EditorGUILayout.PasswordField("VPS Password", vpsPassword);
        winscpPath = EditorGUILayout.TextField("WinSCP Path", winscpPath);

        if (GUILayout.Button("Save Configuration"))
        {
            SaveVPSConfiguration();
        }

        if (GUILayout.Button("Load Configuration"))
        {
            LoadVPSConfiguration();
        }
    }
    private void PreviewAssets(string lessonPath, string archivePath, int currentClass, int currentLesson, Data data)
    {
        int adjustedClass = currentClass - 1;
        int adjustedLesson = currentLesson - 1;

        Data.TypeClass.TypeLesson? entry = FindLessonEntry(data, adjustedClass, adjustedLesson);
        if (entry.HasValue)
        {
            string className = data.typeClasses[adjustedClass].name;
            string lessonName = entry.Value.name;

            // Determine effective archive path
            string effectiveArchivePath = useIgnorePrefabs ? "Assets/.Ignore/Resources/Prefabs" : archivePath;

            // Create base directories if they don't exist
            Directory.CreateDirectory(lessonPath);
            
            // Copy Prefabs
            string prefabSourcePath = Path.Combine(effectiveArchivePath, className, lessonName);
            string prefabTargetPath = Path.Combine(lessonPath, className, lessonName);
            
            if (Directory.Exists(prefabSourcePath))
            {
                CopyDirectory(prefabSourcePath, prefabTargetPath);
            }
            else
            {
                Debug.LogWarning($"Source prefab directory does not exist: {prefabSourcePath}");
                return;
            }

            // Copy StreamingAssets
            if(useIgnorePrefabs)
            {
                string streamingSourcePath = Path.Combine("Assets/.Ignore/StreamingAssets", className, lessonName);
                string streamingTargetPath = Path.Combine("Assets/StreamingAssets", className, lessonName);
                if(Directory.Exists(streamingSourcePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(streamingTargetPath));
                    CopyDirectory(streamingSourcePath, streamingTargetPath);
                }
                
                string textSourcePath = Path.Combine("Assets/.Ignore/Resources/Text", className, lessonName);
                string textTargetPath = Path.Combine("Assets/Resources/Text", className, lessonName);
                if (Directory.Exists(textSourcePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(textTargetPath));
                    CopyDirectory(textSourcePath, textTargetPath);
                }

                string timeSourcePath = Path.Combine("Assets/.Ignore/1_Assets/_Timelines", className, lessonName);
                string timeTargetPath = Path.Combine("Assets/1_Assets/_Timelines", className, lessonName);
                if (Directory.Exists(timeSourcePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(timeTargetPath));
                    CopyDirectory(timeSourcePath, timeTargetPath);
                }
            }
            UpdateGameController(adjustedClass, adjustedLesson);
            AssetDatabase.Refresh();
            Debug.Log("Assets previewed.");
        }
        else
        {
            Debug.LogWarning($"No matching lesson data found for Class {currentClass}, Lesson {currentLesson}");
        }
    }
    private void UpdateResources()
    {
        // Paths for the specific lesson
        string lessonFolder = Path.Combine(lessonPrefabsPath, $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string streamingAssetsPath = Path.Combine("Assets/StreamingAssets", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string textPath = Path.Combine("Assets/Resources/Text", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string timelinesPath = Path.Combine("Assets/1_Assets/_Timelines", $"L{currentClass}", $"L{currentClass}_{currentLesson}");

        // Base backup path with timestamp
        string baseBackupPath = Path.Combine("Assets/.Ignore/Backups", $"L{currentClass}_{currentLesson}_{DateTime.Now:yyyyMMdd_HHmmss}");

        // Archive target paths
        string archiveBasePath = useIgnorePrefabs ? "Assets/.Ignore" : "Assets/5_Archive";
        string archivePrefabsPath = Path.Combine(archiveBasePath, "Resources/Prefabs", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string archiveStreamingPath = Path.Combine(archiveBasePath, "StreamingAssets", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string archiveTextPath = Path.Combine(archiveBasePath, "Resources/Text", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
        string archiveTimelinesPath = Path.Combine(archiveBasePath, "1_Assets/_Timelines", $"L{currentClass}", $"L{currentClass}_{currentLesson}");

        // Back up and update each resource type
        BackupAndCopy(lessonFolder, Path.Combine(baseBackupPath, "Resources/Prefabs"));
        BackupAndCopy(streamingAssetsPath, Path.Combine(baseBackupPath, "StreamingAssets"));
        BackupAndCopy(textPath, Path.Combine(baseBackupPath, "Resources/Text"));
        BackupAndCopy(timelinesPath, Path.Combine(baseBackupPath, "1_Assets/_Timelines"));

        // Update resources in archives
        CopyDirectory(lessonFolder, archivePrefabsPath);
        CopyDirectory(streamingAssetsPath, archiveStreamingPath);
        CopyDirectory(textPath, archiveTextPath);
        CopyDirectory(timelinesPath, archiveTimelinesPath);

        AssetDatabase.Refresh();
        Debug.Log($"Resources for Class {currentClass}, Lesson {currentLesson} updated and backed up.");
    }

    private void BackupAndCopy(string sourceDir, string backupDir)
    {
        if (Directory.Exists(sourceDir))
        {
            Directory.CreateDirectory(backupDir);
            CopyDirectory(sourceDir, backupDir);
        }
    }

    private void CopyDirectory(string sourceDir, string destDir)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                Debug.LogWarning($"Source directory does not exist: {sourceDir}");
                return;
            }

            // Create the destination directory if it doesn't exist
            Directory.CreateDirectory(destDir);

            // Copy files
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    string tempPath = Path.Combine(destDir, file.Name);
                    file.CopyTo(tempPath, true); // Overwrite existing files
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error copying file {file.Name}: {e.Message}");
                }
            }

            // Copy subdirectories
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                try
                {
                    string tempPath = Path.Combine(destDir, subdir.Name);
                    CopyDirectory(subdir.FullName, tempPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error copying directory {subdir.Name}: {e.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in CopyDirectory: {e.Message}");
        }
    }
    private void RevertToBackup()
    {
        // Determine the backup directory for the current lesson
        string backupBaseDir = Path.Combine("Assets/.Ignore/Backups");
        string[] backupDirs = Directory.GetDirectories(backupBaseDir, $"L{currentClass}_{currentLesson}_*");

        if (backupDirs.Length > 0)
        {
            // Choose the most recent backup
            string backupDir = backupDirs.OrderByDescending(d => d).First();

            // Paths for the specific lesson
            string lessonFolder = Path.Combine(lessonPrefabsPath, $"L{currentClass}", $"L{currentClass}_{currentLesson}");
            string streamingAssetsPath = Path.Combine("Assets/StreamingAssets", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
            string textPath = Path.Combine("Assets/Resources/Text", $"L{currentClass}", $"L{currentClass}_{currentLesson}");
            string timelinesPath = Path.Combine("Assets/1_Assets/_Timelines", $"L{currentClass}", $"L{currentClass}_{currentLesson}");

            // Revert each resource type
            RestoreFromBackup(Path.Combine(backupDir, "Resources/Prefabs"), lessonFolder);
            RestoreFromBackup(Path.Combine(backupDir, "StreamingAssets"), streamingAssetsPath);
            RestoreFromBackup(Path.Combine(backupDir, "Resources/Text"), textPath);
            RestoreFromBackup(Path.Combine(backupDir, "1_Assets/_Timelines"), timelinesPath);

            AssetDatabase.Refresh();
            Debug.Log($"Resources for Class {currentClass}, Lesson {currentLesson} reverted from backup.");
        }
        else
        {
            Debug.LogWarning($"No backup found for Class {currentClass}, Lesson {currentLesson}.");
        }
    }

    private void RestoreFromBackup(string backupDir, string targetDir)
    {
        if (Directory.Exists(backupDir))
        {
            // Clear the target directory before restoring
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }
            Directory.CreateDirectory(targetDir);

            // Restore the backup
            CopyDirectory(backupDir, targetDir);
        }
        else
        {
            Debug.LogWarning($"Backup directory does not exist: {backupDir}");
        }
    }
    private void CleanupResources()
    {
        // Clear contents of the Prefabs folder
        ClearDirectoryContents(lessonPrefabsPath);
        AssetDatabase.Refresh();
        Debug.Log("Cleaned up resources.");
    }
    private void ClearDirectoryContents(string directoryPath)
    {
        // Delete all files in the directory
        foreach (string file in Directory.GetFiles(directoryPath))
        {
            File.Delete(file);
            Debug.Log($"Deleted file: {file}");
        }

        // Delete all subdirectories and their contents
        foreach (string subDirectory in Directory.GetDirectories(directoryPath))
        {
            Directory.Delete(subDirectory, true);
            Debug.Log($"Deleted directory: {subDirectory}");
        }
    }
    private Data.TypeClass.TypeLesson? FindLessonEntry(Data data, int adjustedClass, int adjustedLesson)
    {
        if (adjustedClass >= 0 && adjustedClass < data.typeClasses.Length)
        {
            var typeClass = data.typeClasses[adjustedClass];
            var lessons = typeClass.typeLessons;

            if (adjustedLesson >= 0 && adjustedLesson < lessons.Length)
            {
                return lessons[adjustedLesson];
            }
            else
            {
                Debug.LogWarning($"Lesson index {adjustedLesson} is out of bounds for Class {adjustedClass + 1}");
            }
        }
        else
        {
            Debug.LogWarning($"Class index {adjustedClass} is out of bounds");
        }
        return null;
    }
    private void DrawLanguageToggle()
    {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Language/Ngôn ngữ:", GUILayout.Width(150));
        
        string buttonText = isVietnamese ? "Tiếng Việt" : "English";
        if (GUILayout.Button(buttonText, GUILayout.Width(100)))
        {
            isVietnamese = !isVietnamese;
            Repaint();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSingleBuildTab(bool isDataMainSet, bool hasArchivePrefabs)
    {
        GUILayout.Label(GetTranslatedText("Build Specific"), EditorStyles.boldLabel);

        // Track changes in the input fields
        EditorGUI.BeginChangeCheck();

        int newClass = EditorGUILayout.IntField(GetTranslatedText("Current Class"), currentClass);
        int newLesson = EditorGUILayout.IntField(GetTranslatedText("Current Lesson"), currentLesson);

        // Detect if the input fields have changed
        if (EditorGUI.EndChangeCheck())
        {
            // Set the new values and update the timestamp
            currentClass = newClass;
            currentLesson = newLesson;

            // Schedule the update
            pendingUpdate = true;
            updateTimestamp = (float)EditorApplication.timeSinceStartup;
        }

        // Handle Test Build toggle with template switching
        bool newTestBuild = EditorGUILayout.Toggle(GetTranslatedText("Test Build"), isTestBuild);
        if (newTestBuild != isTestBuild)
        {
            isTestBuild = newTestBuild;

            if (isTestBuild)
            {
                // Check if WinSCP is installed before enabling upload
                isWinSCPInstalled = CheckWinSCPInstallation();
                if (!isVpsConfigLoaded || !isWinSCPInstalled)
                {
                    EditorGUILayout.HelpBox("Upload and Test Build are disabled. Ensure VPS info is complete and WinSCP is installed.", MessageType.Warning);
                    isTestBuild = false;
                }
                else
                {
                    uploadBuild = true;  // Automatically enable upload for test builds
                    try
                    {
                        string templatePath = "Assets/WebGLTemplates/Novastars - Test";
                        if (Directory.Exists(templatePath))
                        {
                            PlayerSettings.WebGL.template = "PROJECT:Novastars - Test";
                            Debug.Log($"Template switched to test: {PlayerSettings.WebGL.template}");
                        }
                        else
                        {
                            Debug.LogError($"Test template directory not found: {templatePath}");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to switch to test template: {e.Message}");
                    }
                }
            }
            else
            {
                // Switch back to regular template
                try
                {
                    PlayerSettings.WebGL.template = "PROJECT:Novastars";
                    Debug.Log($"Template switched back to normal: {PlayerSettings.WebGL.template}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to switch back to normal template: {e.Message}");
                }
            }

            // Save the changes
            AssetDatabase.SaveAssets();
        }

        if (!isDataMainSet || !hasArchivePrefabs)
        {
            EditorGUILayout.HelpBox(GetTranslatedText("Cannot build. Please ensure DataMain or DataSummer is set and archive prefabs exist."), MessageType.Warning);
        }
    }

    private void DrawMultiBuildTab()
    {
        GUILayout.Label(GetTranslatedText("Build Many"), EditorStyles.boldLabel);
        startClass = EditorGUILayout.IntField(GetTranslatedText("Start Class"), startClass);
        endClass = EditorGUILayout.IntField(GetTranslatedText("End Class"), endClass);
        startLesson = EditorGUILayout.IntField(GetTranslatedText("Start Lesson"), startLesson);
        endLesson = EditorGUILayout.IntField(GetTranslatedText("End Lesson"), endLesson);

        if (GUILayout.Button(GetTranslatedText("Calculate Build Count")))
        {
            CalculateBuildCount();
        }

        GUILayout.Space(10);
        GUILayout.Label(GetTranslatedText("Build All"), EditorStyles.boldLabel);

        if (GUILayout.Button(GetTranslatedText("Calculate All Builds")))
        {
            CalculateAllBuilds();
        }
    }

    private void CalculateBuildCount()
    {
        buildCount = 0;

        if (lessonData == null)
        {
            errorMessage = "Data ScriptableObject is not assigned.";
            return;
        }

        for (int classNum = startClass; classNum <= endClass; classNum++)
        {
            if (classNum <= lessonData.typeClasses.Length)
            {
                var selectedClass = lessonData.typeClasses[classNum - 1];
                string classPath = Path.Combine(archivePrefabsPath, selectedClass.name);

                if (Directory.Exists(classPath))
                {
                    for (int lessonNum = startLesson; lessonNum <= endLesson; lessonNum++)
                    {
                        if (lessonNum <= selectedClass.typeLessons.Length)
                        {
                            string lessonName = selectedClass.typeLessons[lessonNum - 1].name;
                            string lessonPath = Path.Combine(classPath, lessonName);

                            if (Directory.Exists(lessonPath))
                            {
                                buildCount++;
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"Total builds calculated: {buildCount}");
    }
    private void CalculateAllBuilds()
    {
        buildCount = 0;
        if (lessonData != null)
        {
            foreach (var typeClass in lessonData.typeClasses)
            {
                string classPath = Path.Combine(archivePrefabsPath, typeClass.name);
                if (Directory.Exists(classPath))
                {
                    foreach (var lesson in typeClass.typeLessons)
                    {
                        string lessonPath = Path.Combine(classPath, lesson.name);
                        if (Directory.Exists(lessonPath))
                        {
                            buildCount++;
                        }
                    }
                }
            }
        }
    }
    private void ConfirmAndPerformBuild()
    {
        string actionVerb;
        string additionalInfo = "";
        
        if (uploadBuild)
        {
            if (isTestBuild)
            {
                actionVerb = GetTranslatedText("Are you sure you want to build and upload the test build");
                additionalInfo = $"\n{GetTranslatedText("This will rename the build to lop-1-99")}";
            }
            else
            {
                actionVerb = GetTranslatedText("Are you sure you want to build and upload");
            }
        }
        else
        {
            actionVerb = GetTranslatedText("Are you sure you want to build");
        }

        string buildDescription = selectedTab == 0 
            ? $"{GetTranslatedText("Class")} {currentClass}, {GetTranslatedText("Lesson")} {currentLesson}" 
            : $"{buildCount} {GetTranslatedText("item(s)")}";
            
        string confirmMessage = $"{actionVerb} {buildDescription}?{additionalInfo}";

        string confirmTitle = isTestBuild && uploadBuild 
            ? GetTranslatedText("Confirm Test Build")
            : GetTranslatedText("Confirm Build");

        if (EditorUtility.DisplayDialog(confirmTitle, confirmMessage, GetTranslatedText("Yes"), GetTranslatedText("No")))
        {
            PerformBuild();
        }
    }

    private void PerformBuild()
    {
        if (lessonData == null)
        {
            Debug.LogError("Data ScriptableObject is not assigned.");
            return;
        }

        // Determine the correct archive path based on the toggle state
        string effectiveArchivePath = useIgnorePrefabs ? "Assets/.Ignore/Resources/Prefabs" : archivePrefabsPath;

        if (selectedTab == 0)
        {
            buildCount = 1;
            BuildManager.BuildSpecific(currentClass, currentLesson, uploadBuild, isTestBuild, lessonData, 
                                    vpsHost, vpsPort, vpsUsername, vpsPassword, winscpPath, 
                                    effectiveArchivePath, lessonPrefabsPath, useIgnorePrefabs);
        }
        else
        {
            BuildManager.BuildMany(startClass, endClass, startLesson, endLesson, uploadBuild, lessonData, 
                                vpsHost, vpsPort, vpsUsername, vpsPassword, winscpPath, 
                                effectiveArchivePath, lessonPrefabsPath, useIgnorePrefabs);
        }
    }
    private bool HasMatchingLessonFolders(string path)
    {
        if (lessonData == null)
        {
            errorMessage = "Data ScriptableObject is not assigned.";
            return false;
        }

        foreach (var typeClass in lessonData.typeClasses)
        {
            string classPath = Path.Combine(path, typeClass.name);
            if (Directory.Exists(classPath))
            {
                foreach (var lesson in typeClass.typeLessons)
                {
                    string lessonPath = Path.Combine(classPath, lesson.name);
                    if (Directory.Exists(lessonPath))
                    {
                        errorMessage = "";
                        return true;
                    }
                }
            }
        }

        errorMessage = GetTranslatedText("No matching lesson folders found in the specified path: " + path);
        return false;
    }
    private bool IsDataMainSet()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in canvases)
        {
            GameController gameController = canvas.GetComponentInChildren<GameController>();
            
            if (gameController != null && gameController.data != null)
            {
                //Debug.Log($"Found GameController in Canvas. Data name: {gameController.data.name}");
                if (gameController.data.name == "DataMain" || gameController.data.name == "DataSummer")
                {
                    errorMessage = "";
                    return true;
                }
            }
        }

        errorMessage = GetTranslatedText("DataMain or DataSummer is not set in the Canvas of the current scene.");
        return false;
    }
    [System.Serializable]
    private class VPSConfiguration
    {
        public string host;
        public int port;
        public string username;
        public string password;
        public string winscpPath;
    }

    private void SaveVPSConfiguration()
    {
        VPSConfiguration config = new VPSConfiguration
        {
            host = vpsHost,
            port = vpsPort,
            username = vpsUsername,
            password = vpsPassword,
            winscpPath = winscpPath
        };

        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(configFilePath, json);
        Debug.Log("VPS Configuration Saved to File");
    }

    // Load VPS configuration and manage state
    private void LoadVPSConfiguration()
    {
        
        if (File.Exists(configFilePath))
        {
            string json = File.ReadAllText(configFilePath);
            VPSConfiguration config = JsonUtility.FromJson<VPSConfiguration>(json);

            vpsHost = config.host;
            vpsPort = config.port;
            vpsUsername = config.username;
            vpsPassword = config.password;
            winscpPath = config.winscpPath;

            isVpsConfigLoaded = !string.IsNullOrEmpty(vpsHost) && !string.IsNullOrEmpty(vpsUsername) &&
                                !string.IsNullOrEmpty(vpsPassword) && !string.IsNullOrEmpty(winscpPath);
            
            if (!isVpsConfigLoaded)
            {
                Debug.LogWarning("VPS configuration is incomplete.");
            }
        }
        else
        {
            Debug.LogWarning("VPS Configuration file not found.");
            isVpsConfigLoaded = false;
        }
    }
    private bool CheckWinSCPInstallation()
    {
        string winScpPath = @"C:\Program Files (x86)\WinSCP\WinSCP.exe";
        
        if (File.Exists(winScpPath))
        {
            Debug.Log($"WinSCP found at: {winScpPath}");
            return true;
        }
        
        Debug.LogError($"WinSCP not found at expected path: {winScpPath}");
        return false;
    }
    private string GetTranslatedText(string key)
    {
        if (isVietnamese && translations.ContainsKey(key))
        {
            return translations[key];
        }
        return key;
    }
}