using Mypple_Music.Models;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
namespace Mypple_Music.Service
{
    public class LyricService : ILyricService
    {      
        public ObservableCollection<Lyric> LyricSplitter(string lyric)
        {
            ObservableCollection<Lyric> Lyrics = new();

            // 遍历每一行歌词
            foreach (var line in lyric.Split('\n'))
            {
                // 正则表达式匹配时间戳和歌词
                Match match = Regex.Match(line, @"\[\d{2}:\d{2}\.\d{2}\]");
                if (match.Success)
                {
                    // 提取时间戳
                    string timestamp = line.Substring(0, line.IndexOf(']') + 1)
                        .Replace("[", "")
                        .Replace("]", "");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("00:00:");
                    sb.Append(timestamp);
                    TimeSpan timeSpan = TimeSpan.Parse(sb.ToString());
                    double progress = timeSpan.TotalSeconds;

                    // 提取歌词内容
                    var content = line.Substring(line.IndexOf(']') + 1);
                    if (content != string.Empty)
                        Lyrics.Add(new Lyric() { Content = content, TimeSpan = progress });

                    // 匹配原文和中文

                    // 如果有两个匹配结果，则分别添加到原文和中文的字符串中
                    //if (newContent.Length == 2)
                    //{
                    //    Lyrics.Add(new Lyric() { Content = newContent[0], TimeSpan = progress, Translation = newContent[1]});
                    //}
                    //else
                    //{
                    //    Lyrics.Add(new Lyric() { Content = newContent[0], TimeSpan = progress });

                    //}
                }
            }
            return Lyrics;
        }
    }
}
