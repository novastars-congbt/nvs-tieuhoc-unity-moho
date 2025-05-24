// Assets/Test_TieuHoc/Editor/Validators/TimelineCleaner.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Timeline;
using UnityEditor.Timeline;
#endif

namespace Test_TieuHoc.Validation
{
    /// <summary>
    /// Validator tìm và xóa các track rỗng trong timeline
    /// </summary>
    public class TimelineCleaner : IValidator
    {
        public string Name => "Kiểm tra Timeline";
        public string Description => "Kiểm tra và dọn dẹp timeline trong project";
        
        // Thiết lập có thể tùy chỉnh
        public bool SearchInCurrentSceneOnly = true;
        public bool IncludeSubEmptyTracks = true;
        
        private bool checkInactiveObjects = true;
        private float maxTimelineLength = 100f; // Maximum timeline length in seconds
        
        public bool CheckInactiveObjects
        {
            get => checkInactiveObjects;
            set => checkInactiveObjects = value;
        }

        public float MaxTimelineLength
        {
            get => maxTimelineLength;
            set => maxTimelineLength = value;
        }
        
        // Cấu trúc dữ liệu lưu trữ thông tin track rỗng
        public class EmptyTrackInfo
        {
            public TimelineAsset timeline;
            public TrackAsset track;
            public string timelineName;
            public string trackPath;
        }
        
        /// <summary>
        /// Kiểm tra tất cả timeline để tìm các track rỗng
        /// </summary>
        public List<ValidationIssue> Validate()
        {
            var issues = new List<ValidationIssue>();
            
            // Find all Timeline assets in the project
            string[] timelineGuids = AssetDatabase.FindAssets("t:TimelineAsset");
            foreach (string guid in timelineGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TimelineAsset timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
                if (timeline == null) continue;

                // Check for empty tracks
                foreach (var track in timeline.GetOutputTracks())
                {
                    if (track.isEmpty)
                    {
                        var issue = new ValidationIssue();
                        issue.target = timeline;
                        issue.message = $"Timeline '{timeline.name}' có track rỗng: {track.name}";
                        issue.severity = ValidationSeverity.Warning;
                        issue.canAutoFix = true;
                        issues.Add(issue);
                    }

                    // Check for duplicate tracks
                    var clips = track.GetClips();
                    var clipTimes = new Dictionary<double, int>();
                    foreach (var clip in clips)
                    {
                        if (clipTimes.ContainsKey(clip.start))
                        {
                            var issue = new ValidationIssue();
                            issue.target = timeline;
                            issue.message = $"Timeline '{timeline.name}' có clip trùng thời điểm tại {clip.start}s trong track {track.name}";
                            issue.severity = ValidationSeverity.Warning;
                            issue.canAutoFix = false;
                            issues.Add(issue);
                        }
                        else
                        {
                            clipTimes[clip.start] = 1;
                        }
                    }
                }

                // Check timeline length
                double duration = timeline.duration;
                if (duration > maxTimelineLength)
                {
                    var issue = new ValidationIssue();
                    issue.target = timeline;
                    issue.message = $"Timeline '{timeline.name}' dài hơn {maxTimelineLength}s (hiện tại: {duration:F1}s)";
                    issue.severity = ValidationSeverity.Warning;
                    issue.canAutoFix = false;
                    issues.Add(issue);
                }
            }
            
            // Check for satia (normal) settings
            var playableDirectors = ValidatorUtils.FindObjectsOfType<PlayableDirector>(checkInactiveObjects);
            foreach (var director in playableDirectors)
            {
                if (director.playableAsset is TimelineAsset)
                {
                    // Check initial time
                    if (director.initialTime != 0)
                    {
                        var issue = new ValidationIssue();
                        issue.target = director;
                        issue.message = $"Timeline Director '{director.name}' có initial time khác 0 ({director.initialTime}s)";
                        issue.severity = ValidationSeverity.Warning;
                        issue.canAutoFix = true;
                        issues.Add(issue);
                    }

                    // Check wrap mode
                    if (director.extrapolationMode != DirectorWrapMode.None)
                    {
                        var issue = new ValidationIssue();
                        issue.target = director;
                        issue.message = $"Timeline Director '{director.name}' không set wrap mode là None";
                        issue.severity = ValidationSeverity.Warning;
                        issue.canAutoFix = true;
                        issues.Add(issue);
                    }
                }
            }
            
            return issues;
        }
        
        /// <summary>
        /// Sửa tất cả các vấn đề về track rỗng
        /// </summary>
        public void FixAll()
        {
            var issues = Validate();
            foreach (var issue in issues)
            {
                Fix(issue);
            }
        }
        
        /// <summary>
        /// Sửa một vấn đề cụ thể
        /// </summary>
        public void Fix(ValidationIssue issue)
        {
            if (issue.target is TimelineAsset timeline && issue.message.Contains("track rỗng"))
            {
                var tracks = timeline.GetOutputTracks();
                foreach (var track in tracks)
                {
                    if (track.isEmpty)
                    {
                        timeline.DeleteTrack(track);
                    }
                }
            }
            else if (issue.target is PlayableDirector director)
            {
                if (issue.message.Contains("initial time"))
                {
                    director.initialTime = 0;
                }
                if (issue.message.Contains("wrap mode"))
                {
                    director.extrapolationMode = DirectorWrapMode.None;
                }
            }
        }
        
#if UNITY_2019_1_OR_NEWER
        /// <summary>
        /// Tìm tất cả track rỗng trong project hoặc scene hiện tại
        /// </summary>
        private List<EmptyTrackInfo> FindEmptyTracks()
        {
            List<EmptyTrackInfo> emptyTracks = new List<EmptyTrackInfo>();
            
            // Tìm tất cả TimelineAsset trong project hoặc scene
            List<TimelineAsset> timelineAssets = new List<TimelineAsset>();
            
            if (SearchInCurrentSceneOnly)
            {
                // Tìm trong scene hiện tại
                var playableDirectors = ValidatorUtils.FindObjectsOfType<PlayableDirector>(true);
                foreach (var director in playableDirectors)
                {
                    if (director.playableAsset is TimelineAsset timeline)
                    {
                        timelineAssets.Add(timeline);
                    }
                }
            }
            else
            {
                // Tìm trong toàn bộ project
                string[] guids = AssetDatabase.FindAssets("t:TimelineAsset");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    TimelineAsset timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);
                    if (timeline != null)
                    {
                        timelineAssets.Add(timeline);
                    }
                }
            }
            
            // Kiểm tra các track trong mỗi timeline
            foreach (var timeline in timelineAssets)
            {
                ProcessTimelineTracks(timeline, timeline.GetRootTracks().ToList(), "", emptyTracks);
            }
            
            return emptyTracks;
        }
        
        /// <summary>
        /// Xử lý đệ quy các track trong timeline để tìm track rỗng
        /// </summary>
        private void ProcessTimelineTracks(TimelineAsset timeline, List<TrackAsset> tracks, string parentPath, List<EmptyTrackInfo> results)
        {
            foreach (var track in tracks)
            {
                string trackPath = string.IsNullOrEmpty(parentPath) ? track.name : $"{parentPath}/{track.name}";
                
                // Kiểm tra xem track có rỗng không
                bool isEmpty = track.GetClips().Count() == 0 && track.GetMarkers().Count() == 0;
                
                if (isEmpty)
                {
                    // Thêm vào danh sách kết quả
                    results.Add(new EmptyTrackInfo
                    {
                        timeline = timeline,
                        track = track,
                        timelineName = timeline.name,
                        trackPath = trackPath
                    });
                }
                
                // Xử lý các track con nếu được cấu hình
                if (IncludeSubEmptyTracks)
                {
                    var childTracks = track.GetChildTracks().ToList();
                    if (childTracks.Count > 0)
                    {
                        ProcessTimelineTracks(timeline, childTracks, trackPath, results);
                    }
                }
            }
        }
        
        /// <summary>
        /// Xóa track rỗng được chỉ định
        /// </summary>
        private void RemoveEmptyTrack(EmptyTrackInfo trackInfo)
        {
            TimelineAsset timeline = trackInfo.timeline;
            TrackAsset track = trackInfo.track;
            
            // Đánh dấu timeline là dirty để có thể undo
            Undo.RegisterCompleteObjectUndo(timeline, "Remove Empty Timeline Track");
            
            // Xóa track
            bool removed = timeline.DeleteTrack(track);
            
            if (removed)
            {
                Debug.Log($"Đã xóa track rỗng: [{trackInfo.timelineName}] - {trackInfo.trackPath}");
                EditorUtility.SetDirty(timeline);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning($"Không thể xóa track: [{trackInfo.timelineName}] - {trackInfo.trackPath}");
            }
        }
#endif
    }
}