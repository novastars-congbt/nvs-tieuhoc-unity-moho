using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NareshBisht
{
    class SubtitleEntry
    {
        public int index { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public string text { get; set; }
    }

    class SubtitleParser
    {
        public static List<SubtitleEntry> ParseSrt(string srt)
        {
            var entries = new List<SubtitleEntry>();
            var blocks = Regex.Split(srt.Trim(), @"\r?\n\r?\n");

            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                if (lines.Length >= 3)
                {
                    var index = int.Parse(lines[0].Trim());

                    var times = lines[1].Split(new[] { " --> " }, StringSplitOptions.None);
                    var start = TimeSpan.Parse(times[0].Replace(',', '.'));
                    var end = TimeSpan.Parse(times[1].Replace(',', '.'));

                    var text = string.Join("\n", lines, 2, lines.Length - 2);

                    entries.Add(new SubtitleEntry
                    {
                        index = index,
                        startTime = start,
                        endTime = end,
                        text = text
                    });
                }
            }

            return entries;
        }
    }   
}