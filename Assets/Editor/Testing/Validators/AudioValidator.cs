using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

namespace Test_TieuHoc.Validation
{
    public class AudioValidator : IValidator
    {
        // Cache the audio clips to avoid repeated loading
        private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
        
        public string Name => "Kiểm tra Audio";
        public string Description => "Kiểm tra các thiết lập âm thanh trong project";
        
        private bool checkInactiveObjects = true;
        
        public bool CheckInactiveObjects
        {
            get => checkInactiveObjects;
            set => checkInactiveObjects = value;
        }

        public List<ValidationIssue> Validate()
        {
            var issues = new List<ValidationIssue>();
            
            // Process in batches to avoid freezing
            ValidateAudioSources(issues);
            ValidateAudioClips(issues);
            
            return issues;
        }
        
        private void ValidateAudioSources(List<ValidationIssue> issues)
        {
            var audioSources = Object.FindObjectsOfType<AudioSource>(checkInactiveObjects);
            
            // Process in smaller batches
            const int batchSize = 20;
            for (int i = 0; i < audioSources.Length; i += batchSize)
            {
                int endIndex = Mathf.Min(i + batchSize, audioSources.Length);
                for (int j = i; j < endIndex; j++)
                {
                    var audioSource = audioSources[j];
                    
                    // Skip null components (can happen with broken references)
                    if (audioSource == null) continue;
                    
                    // Check Master Volume
                    if (audioSource.volume > 1f)
                    {
                        var issue = new ValidationIssue();
                        issue.target = audioSource;
                        issue.message = $"AudioSource '{audioSource.name}' có volume > 1 ({audioSource.volume})";
                        issue.severity = ValidationSeverity.Error;
                        issue.canAutoFix = true;
                        issues.Add(issue);
                    }
                    
                    // Check 3D Sound Settings
                    if (audioSource.spatialBlend > 0)
                    {
                        // Check for HandleRake preset
                        var audioMixer = audioSource.outputAudioMixerGroup;
                        if (audioMixer == null || !audioMixer.name.Contains("HandleRake"))
                        {
                            var issue = new ValidationIssue();
                            issue.target = audioSource;
                            issue.message = $"AudioSource 3D '{audioSource.name}' không sử dụng HandleRake mixer group";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = false;
                            issues.Add(issue);
                        }
                    }
                }
                
                // Allow UI to update between batches
                if (i + batchSize < audioSources.Length)
                {
                    EditorUtility.DisplayProgressBar(
                        "Kiểm tra Audio Sources", 
                        $"Đang kiểm tra AudioSource {i + batchSize}/{audioSources.Length}", 
                        (float)(i + batchSize) / audioSources.Length);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }
        
        private void ValidateAudioClips(List<ValidationIssue> issues)
        {
            // Load the audio clip cache if empty
            if (audioClipCache.Count == 0)
            {
                CacheAudioClips();
            }
            
            // Process in smaller batches
            string[] audioFiles = audioClipCache.Keys.ToArray();
            const int batchSize = 10;
            
            for (int i = 0; i < audioFiles.Length; i += batchSize)
            {
                int endIndex = Mathf.Min(i + batchSize, audioFiles.Length);
                for (int j = i; j < endIndex; j++)
                {
                    string path = audioFiles[j];
                    AudioClip clip = audioClipCache[path];
                    
                    if (clip == null) continue;
                    
                    AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                    if (importer != null)
                    {
                        // Check format
                        if (!path.EndsWith(".mp3"))
                        {
                            var issue = new ValidationIssue();
                            issue.target = clip;
                            issue.message = $"Audio clip '{clip.name}' không phải định dạng mp3";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = false;
                            issues.Add(issue);
                        }
                        
                        // Check quality settings
                        var settings = importer.defaultSampleSettings;
                        if (settings.sampleRateSetting != AudioSampleRateSetting.OverrideSampleRate ||
                            settings.sampleRateOverride != 44100)
                        {
                            var issue = new ValidationIssue();
                            issue.target = clip;
                            issue.message = $"Audio clip '{clip.name}' không đúng sample rate (44.1kHz)";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = true;
                            issues.Add(issue);
                        }
                        
                        // Check channels (mono)
                        if (clip.channels != 1)
                        {
                            var issue = new ValidationIssue();
                            issue.target = clip;
                            issue.message = $"Audio clip '{clip.name}' không phải mono (hiện tại: {clip.channels} channels)";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = true;
                            issues.Add(issue);
                        }
                    }
                }
                
                // Allow UI to update between batches
                if (i + batchSize < audioFiles.Length)
                {
                    EditorUtility.DisplayProgressBar(
                        "Kiểm tra Audio Clips", 
                        $"Đang kiểm tra AudioClip {i + batchSize}/{audioFiles.Length}", 
                        (float)(i + batchSize) / audioFiles.Length);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }
        
        private void CacheAudioClips()
        {
            audioClipCache.Clear();
            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip");
            
            foreach (string guid in audioGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null)
                {
                    audioClipCache[path] = clip;
                }
            }
        }

        public void Fix(ValidationIssue issue)
        {
            if (issue.target is AudioSource audioSource)
            {
                if (issue.message.Contains("volume"))
                {
                    audioSource.volume = 1f;
                }
            }
            else if (issue.target is AudioClip clip)
            {
                string path = AssetDatabase.GetAssetPath(clip);
                AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
                if (importer != null)
                {
                    var settings = importer.defaultSampleSettings;
                    bool needsReimport = false;

                    if (issue.message.Contains("sample rate"))
                    {
                        settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                        settings.sampleRateOverride = 44100;
                        needsReimport = true;
                    }
                    
                    if (issue.message.Contains("mono"))
                    {
                        settings.loadType = AudioClipLoadType.DecompressOnLoad;
                        importer.forceToMono = true;
                        needsReimport = true;
                    }

                    if (needsReimport)
                    {
                        importer.defaultSampleSettings = settings;
                        importer.SaveAndReimport();
                    }
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