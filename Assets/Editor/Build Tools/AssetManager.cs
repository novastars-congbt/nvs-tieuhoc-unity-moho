using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetManager
{
    // Paths are now passed as arguments to the functions
    public static bool MovePrefabsToArchive(string sourcePath, string targetPath)
    {
        if (!HasLessonPrefabs(sourcePath))
        {
            Debug.LogWarning("No prefabs found in the Resources folder.");
            return false;
        }

        MoveAllDirectories(sourcePath, targetPath); // Process all directories
        AssetDatabase.Refresh();
        return true;
    }

    public static bool MovePrefabsFromArchive(string sourcePath, string targetPath)
    {
        if (!HasRelevantArchivePrefabs(sourcePath))
        {
            Debug.LogWarning("No prefabs found in the archive.");
            return false;
        }

        MoveAllDirectories(sourcePath, targetPath); // Process all directories
        AssetDatabase.Refresh();
        return true;
    }

    private static void MoveAllDirectories(string sourcePath, string targetPath)
    {
        if (!Directory.Exists(sourcePath))
        {
            Debug.LogWarning($"Source path does not exist: {sourcePath}");
            return;
        }

        Directory.CreateDirectory(targetPath);

        string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.TopDirectoryOnly);
        foreach (string dir in directories)
        {
            string dirName = new DirectoryInfo(dir).Name;
            string targetDir = Path.Combine(targetPath, dirName);

            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true); // Delete existing directory
            }

            Directory.Move(dir, targetDir);
            Debug.Log($"Moved directory from {dir} to {targetDir}");

            // Delete .meta file of the moved directory
            string metaFilePath = dir + ".meta";
            if (File.Exists(metaFilePath))
            {
                File.Delete(metaFilePath);
                Debug.Log($"Deleted meta file: {metaFilePath}");
            }
        }
    }

    // Updated to use the provided path and check for ANY subdirectories
    private static bool HasLessonPrefabs(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        string[] classFolders = Directory.GetDirectories(path);

        foreach (string classFolder in classFolders)
        {
            if (Directory.GetDirectories(classFolder).Length > 0)
            {
                return true;
            }
        }

        return false;
    }

    // Updated to use the provided path and check for ANY subdirectories
    private static bool HasRelevantArchivePrefabs(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        string[] classFolders = Directory.GetDirectories(path);

        foreach (string classFolder in classFolders)
        {
            if (Directory.GetDirectories(classFolder).Length > 0)
            {
                return true;
            }
        }

        return false;
    }
}
