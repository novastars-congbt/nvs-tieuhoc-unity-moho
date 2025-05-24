using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Debug = UnityEngine.Debug;

public static class BuildManager
{
    public static void BuildSpecific(int classNum, int lessonNum, bool upload, bool isTestBuild, Data lessonData, 
                                    string vpsHost, int vpsPort, string vpsUsername, string vpsPassword, 
                                    string winscpPath, string archiveBasePath, string prefabBasePath, bool useIgnorePrefabs)
    {
        if (lessonData == null)
        {
            Debug.LogError("Data ScriptableObject is not assigned.");
            return;
        }

        if (classNum <= 0 || classNum > lessonData.typeClasses.Length)
        {
            Debug.LogError($"Class with index {classNum} is out of range.");
            return;
        }
        
        var selectedClass = lessonData.typeClasses[classNum - 1];
        var lesson = selectedClass.typeLessons[lessonNum - 1];
        string lessonName = lesson.name;
        string lessonNumFormatted = lessonNum.ToString("D2");
        string folderName = $"{selectedClass.scheme}-{lessonNumFormatted}";

        if (isTestBuild)
        {
            folderName = $"{selectedClass.scheme}-99";
        }
        
        string archivePath = Path.Combine(archiveBasePath, selectedClass.name, lessonName);
        string prefabPath = Path.Combine(prefabBasePath, selectedClass.name, lessonName);

        if (!Directory.Exists(archivePath))
        {
            Debug.LogError($"No matching archive found for class {classNum}, lesson {lessonNumFormatted}");
            return;
        }
        // Move StreamingAssets content
        if(useIgnorePrefabs)
            MoveIgnoreAssets(selectedClass.name, lessonName);

        CopyPrefabsToResources(archivePath, prefabPath);
        UpdateGameController(classNum - 1, lessonNum - 1);
        string buildPath = Path.Combine("Builds", folderName);

        BuildWebGL(buildPath);

        // Use the provided paths for cleanup
        CleanUpAssets(Path.Combine(prefabBasePath, selectedClass.name), Path.Combine(prefabBasePath, selectedClass.name, lessonName), buildPath, selectedClass.name, lessonName, useIgnorePrefabs);
        if (upload)
        {
            ManageBuildVersionAndUpload(buildPath, folderName, true, vpsHost, vpsPort, vpsUsername, vpsPassword, winscpPath);
        }

        Thread.Sleep(30000);
    }

    public static void BuildAll(bool upload, Data lessonData, string vpsHost, int vpsPort, string vpsUsername, string vpsPassword, string winscpPath, string archiveBasePath, string prefabBasePath, bool useIgnorePrefabs)
    {
        foreach (var typeClass in lessonData.typeClasses)
        {
            int classNum = Array.IndexOf(lessonData.typeClasses, typeClass) + 1;
            foreach (var lesson in typeClass.typeLessons)
            {
                int lessonNum = Array.IndexOf(typeClass.typeLessons, lesson) + 1;
                BuildSpecific(classNum, lessonNum, upload, false, lessonData, vpsHost, vpsPort, vpsUsername, vpsPassword, winscpPath, archiveBasePath, prefabBasePath, useIgnorePrefabs);
            }
        }
    }

    public static void BuildMany(int startClass, int endClass, int startLesson, int endLesson, bool upload, Data lessonData, string vpsHost, int vpsPort, string vpsUsername, string vpsPassword, string winscpPath, string archiveBasePath, string prefabBasePath, bool useIgnorePrefabs)
    {
        for (int classNum = startClass; classNum <= endClass; classNum++)
        {
            for (int lessonNum = startLesson; lessonNum <= endLesson; lessonNum++)
            {
                BuildSpecific(classNum, lessonNum, upload, false, lessonData, vpsHost, vpsPort, vpsUsername, vpsPassword, winscpPath, archiveBasePath, prefabBasePath, useIgnorePrefabs);
            }
        }
    }

    private static void UpdateGameController(int classNum, int lessonNum)
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

    private static void BuildWebGL(string buildPath)
    {
        Directory.CreateDirectory(buildPath);
        string[] scenes = GetScenesFromBuildSettings();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    private static void MoveIgnoreAssets(string className, string lessonName)
    {
        string ignoreStreamingAssetsPath = Path.Combine("Assets/.Ignore/StreamingAssets");
        string targetStreamingAssetsPath = Path.Combine("Assets/StreamingAssets");

        if (Directory.Exists(ignoreStreamingAssetsPath))
        {
            CopyPrefabsToResources(ignoreStreamingAssetsPath, targetStreamingAssetsPath);
            Debug.Log("Moved contents from .Ignore/StreamingAssets to StreamingAssets.");
        }
        string textSourcePath = Path.Combine("Assets/.Ignore/Resources/Text", className, lessonName); // Adjust path as needed
        string textTargetPath = Path.Combine("Assets/Resources/Text", className, lessonName); // Adjust path as needed
        if (Directory.Exists(textSourcePath) && !Directory.Exists(textTargetPath))
            CopyPrefabsToResources(textSourcePath, textTargetPath);

        string timeSourcePath = Path.Combine("Assets/.Ignore/1_Assets/_Timelines", className, lessonName); // Adjust path as needed
        string timeTargetPath = Path.Combine("Assets/1_Assets/_Timelines", className, lessonName); // Adjust path as needed
        if (Directory.Exists(timeSourcePath) && !Directory.Exists(timeTargetPath))
            CopyPrefabsToResources(timeSourcePath, timeTargetPath);
    }

    private static void CopyPrefabsToResources(string archivePath, string prefabPath)
    {
        Directory.CreateDirectory(prefabPath);
        string[] files = Directory.GetFiles(archivePath, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string targetFile = file.Replace(archivePath, prefabPath);
            string targetDirectory = Path.GetDirectoryName(targetFile);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            File.Copy(file, targetFile, true);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Copied prefabs from {archivePath} to {prefabPath}");
    }

    private static void CleanUpAssets(string classPath, string lessonPath, string buildDirectory, string currentClassName, string currentLessonName, bool useIgnorePrefabs)
    {
        // Clean up lesson folder
        if (Directory.Exists(lessonPath))
        {
            Directory.Delete(lessonPath, true);
            DeleteMetaFile(lessonPath);
            Debug.Log($"Deleted lesson folder and meta file: {lessonPath}");
        }

        // Check if class folder is empty, then delete
        if (Directory.Exists(classPath) && Directory.GetDirectories(classPath).Length == 0)
        {
            Directory.Delete(classPath, true);
            DeleteMetaFile(classPath);
            Debug.Log($"Deleted class folder and meta file: {classPath}");
        }

        // Clean up StreamingAssets
        string streamingAssetsPath = Path.Combine(buildDirectory, "StreamingAssets");
        if(useIgnorePrefabs){
            string textAssetsPath = Path.Combine("Assets/Resources/Text", currentClassName, currentLessonName);
            string timelineAssetsPath = Path.Combine("Assets/1_Assets/_Timelines", currentClassName, currentLessonName);

            // Clean up lesson assets folders
            if (Directory.Exists(textAssetsPath))
            {
                Directory.Delete(textAssetsPath, true);
                DeleteMetaFile(textAssetsPath);
                Debug.Log($"Deleted text folder and meta file: {textAssetsPath}");
            }

            if (Directory.Exists(timelineAssetsPath))
            {
                Directory.Delete(timelineAssetsPath, true);
                DeleteMetaFile(timelineAssetsPath);
                Debug.Log($"Deleted timeline folder and meta file: {timelineAssetsPath}");
            }
        }
        if (Directory.Exists(streamingAssetsPath))
        {
            string[] classFolders = Directory.GetDirectories(streamingAssetsPath);
            foreach (string classFolder in classFolders)
            {
                string folderName = new DirectoryInfo(classFolder).Name;

                if (folderName != currentClassName)
                {
                    // Delete class folders not matching the current class name
                    Directory.Delete(classFolder, true);
                    DeleteMetaFile(classFolder);
                    Debug.Log($"Deleted unused class folder: {classFolder}");
                }
                else
                {
                    // If the folder matches the current class name, delete non-matching lesson folders
                    string[] lessonFolders = Directory.GetDirectories(classFolder);
                    foreach (string lessonFolder in lessonFolders)
                    {
                        string lessonFolderName = new DirectoryInfo(lessonFolder).Name;

                        if (lessonFolderName != currentLessonName)
                        {
                            Directory.Delete(lessonFolder, true);
                            DeleteMetaFile(lessonFolder);
                            Debug.Log($"Deleted unused lesson folder: {lessonFolder}");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("StreamingAssets directory does not exist in the build.");
        }

        AssetDatabase.Refresh();
    }

    private static void DeleteMetaFile(string folderPath)
    {
        string metaFilePath = folderPath.TrimEnd(Path.DirectorySeparatorChar) + ".meta";
        if (File.Exists(metaFilePath))
        {
            File.Delete(metaFilePath);
            Debug.Log($"Deleted meta file: {metaFilePath}");
        }
    }
    // Update ManageBuildVersionAndUpload to accept VPS parameters
    private static void ManageBuildVersionAndUpload(string buildDirectory, string folderName, bool upload, string host, int port, string username, string password, string winscpPath)
    {
        string remotePath = "/secure_site_simss_admin/www/html/portal-lms/storage/app/public/webgl";
        string versionDirectory = Path.Combine(buildDirectory, "Build");
        Directory.CreateDirectory(versionDirectory);
        string newVersion = DownloadAndDetermineVersion(buildDirectory, host, port, username, password, remotePath, folderName, winscpPath);

        UpdateVersionFile(versionDirectory, newVersion);

        Debug.Log($"Version updated to {newVersion}");

        if (upload)
        {
            UploadBuildToVPS(buildDirectory, host, port, username, password, remotePath, winscpPath);
        }
    }

    private static string DownloadAndDetermineVersion(string buildDirectory, string host, int port, string username, string password, string remotePath, string folderName, string winscpPath)
    {
        string currentVersion = "1.0";
        string versionFileName = "version.json";
        string localVersionPath = Path.Combine(buildDirectory, "Build", versionFileName);
        string scriptPath = Path.Combine(buildDirectory, "list_dir.txt");

        string[] scriptLines = new[]
        {
            $"open sftp://{username}:{password}@{host}:{port}",
            $"cd \"{remotePath}/{folderName}/Build\"",
            $"get \"{versionFileName}\" \"{localVersionPath}\"",
            "exit"
        };

        File.WriteAllLines(scriptPath, scriptLines);
        ExecuteWinSCPCommand(winscpPath, scriptPath);

        if (File.Exists(localVersionPath))
        {
            try
            {
                string json = File.ReadAllText(localVersionPath);
                VersionInfo versionInfo = JsonUtility.FromJson<VersionInfo>(json);
                string[] versionParts = versionInfo.gameVersion.Split('.');
                int major = int.Parse(versionParts[0]);
                int minor = int.Parse(versionParts[1]);

                minor += 1;
                if (minor >= 10)
                {
                    major += 1;
                    minor = 0;
                }

                currentVersion = $"{major}.{minor}";
                Debug.Log($"Current version {versionInfo.gameVersion} updated to {currentVersion}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse version file: {e.Message}");
            }
            finally
            {
                File.Delete(localVersionPath);
            }
        }
        else
        {
            Debug.LogWarning($"No version.json file found on server. Starting with default version 1.0.");
        }

        File.Delete(scriptPath);
        return currentVersion;
    }

    private static void UpdateVersionFile(string buildDirectory, string newVersion)
    {
        string destinationPath = Path.Combine(buildDirectory, "version.json");
        VersionInfo versionInfo = new VersionInfo
        {
            unityVersion = Application.unityVersion,
            gameVersion = newVersion,
            buildDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            description = "Automated build update"
        };

        string json = JsonUtility.ToJson(versionInfo, true);
        File.WriteAllText(destinationPath, json);
        Debug.Log("version.json has been updated at: " + destinationPath);
    }

    private static void UploadBuildToVPS(string buildDirectory, string host, int port, string username, string password, string remotePath, string winscpPath)
    {
        string scriptPath = Path.Combine(buildDirectory, "upload.txt");

        string parentDirectory = Directory.GetParent(buildDirectory).FullName;
        string folderName = new DirectoryInfo(buildDirectory).Name;

        string[] scriptLines = new[]
        {
            $"open sftp://{username}:{password}@{host}:{port}",
            $"cd \"{remotePath}\"",
            $"lcd \"{parentDirectory}\"",
            $"put \"{folderName}\"",
            "exit"
        };

        File.WriteAllLines(scriptPath, scriptLines);
        Debug.Log($"WinSCP script created at: {scriptPath}");
        ExecuteWinSCPCommand(winscpPath, scriptPath);

        try
        {
            File.Delete(scriptPath);
            Debug.Log($"Deleted local WinSCP script file: {scriptPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete local WinSCP script file: {e.Message}");
        }
    }

    private static void ExecuteWinSCPCommand(string winscpPath, string scriptPath)
    {
        try
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = winscpPath,
                    Arguments = $"/script=\"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            Debug.Log($"WinSCP Output: {result}");
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"WinSCP Error: {error}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to execute WinSCP command: {e.Message}");
        }
    }

    private static string[] GetScenesFromBuildSettings()
    {
        return EditorBuildSettings.scenes
                                   .Where(scene => scene.enabled)
                                   .Select(scene => scene.path)
                                   .ToArray();
    }

    [Serializable]
    public class VersionInfo
    {
        public string unityVersion;
        public string gameVersion;
        public string buildDate;
        public string description;
    }
}
