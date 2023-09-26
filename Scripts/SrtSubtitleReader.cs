
using UdonLab.Toolkit;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.VideoSubtitle
{
    public class SrtSubtitleReader : UdonSharpBehaviour
    {
        // /// <summary>
        // /// Udon Array Plus
        // /// </summary>
        // [Header("Udon Array Plus")]
        // [SerializeField] public UdonArrayPlus udonArrayPlus;
        /// <summary>
        /// 字幕
        /// </summary>
        [Header("字幕")]
        public VideoSubtitle[] subtitles;
        void Start()
        {
            // if (udonArrayPlus == null)
            // {
            //     Debug.LogError("Udon Array Plus is null");
            //     return;
            // }
            if (subtitles == null)
            {
                Debug.LogError("subtitles is null");
                return;
            }
            for (int i = 0; i < subtitles.Length; i++)
            {
                if (subtitles[i].subtitleText.Length != 0 && subtitles[i].subtitleStartTime.Length != 0 && subtitles[i].subtitleEndTime.Length != 0)
                    continue;
                ReadSrtFile(subtitles[i]);
            }
        }
        void ReadSrtFile(VideoSubtitle subtitle)
        {
            // 1
            // 00:00:31,660 --> 00:00:34,160
            // “长亭外”
            // ♪Outside the long corridor♪
            //
            // 2
            // 00:00:34,660 --> 00:00:37,540
            // “古道边”
            // ♪Along the ancient road♪
            //
            // 3
            // 00:00:37,910 --> 00:00:44,370
            // “芳草碧连天”
            // ♪The green grass seemed to connect with the sky.♪
            //  --> 853译 <--
            //
            // 4
            // 00:00:45,040 --> 00:00:51,500
            // “晚风拂柳笛声残”
            // ♪Under the gentle night breeze,
            // swung the willows and resonated the melancholic melody from an old flute♪
            //
            // 5
            // 00:00:51,540 --> 00:00:57,790
            // “夕阳山外山”
            // ♪as sun sets among the mountains.♪
            //
            // 以下省略
            var _srtFile = subtitle.subtitleFile;
            if (_srtFile == null)
                return;
            var _subtitleText = new string[0];
            var _subtitleStartTime = new float[0];
            var _subtitleEndTime = new float[0];
            string[] lines = _srtFile.text.Split('\n');
            if (_srtFile.text.Contains("\r\n"))
            {
                lines = _srtFile.text.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
            }
            float startTime = -1f;
            float endTime = -1f;
            string text = "";
            bool haveNumber = lines[0] == "1";
            if (haveNumber)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (int.TryParse(line, out int _number)
                    && i + 1 < lines.Length
                    && lines[i + 1].Contains(" --> ")
                    && char.IsDigit(lines[i + 1][0]) && char.IsDigit(lines[i + 1][lines[i + 1].Length - 1]))
                    {
                        string[] times = lines[i + 1].Split(new string[] { " --> " }, System.StringSplitOptions.None);
                        if (times.Length == 2)
                        {
                            startTime = ParseTime(times[0]);
                            endTime = ParseTime(times[1]);
                        }
                        i++;
                    }
                    else if (line.Contains(" --> ")
                    && char.IsDigit(line[0]) && char.IsDigit(line[line.Length - 1]))
                    // && int.TryParse(lines[i - 1], out int _number1))
                    {
                        string[] times = line.Split(new string[] { " --> " }, System.StringSplitOptions.None);
                        if (times.Length == 2)
                        {
                            startTime = ParseTime(times[0]);
                            endTime = ParseTime(times[1]);
                        }
                    }
                    else if (line == ""
                    // && i + 2 < lines.Length
                    // && int.TryParse(lines[i + 1], out int _number3)
                    && (i + 2 < lines.Length
                    && lines[i + 2].Contains(" --> ")
                    && char.IsDigit(lines[i + 2][0]) && char.IsDigit(lines[i + 2][lines[i + 2].Length - 1]))
                    || (i + 2 == lines.Length && string.IsNullOrEmpty(lines[i + 1]))
                    || (i + 3 == lines.Length && string.IsNullOrEmpty(lines[i + 1]) && string.IsNullOrEmpty(lines[i + 2])))
                    {
                        // if (i + 2 < lines.Length)
                        // {
                        //     if (lines[i + 1] == (_subtitleText.Length + 1).ToString()
                        //     && lines[i + 2].Contains(" --> "))
                        //     {
                        //         // nothing
                        //     }
                        //     // else
                        //     // {
                        //     //     if (text != "")
                        //     //         text += "\n";
                        //     //     text += line;
                        //     //     continue;
                        //     // }
                        // }
                        if (startTime >= 0f && endTime >= 0f && text != "")
                        {
                            _subtitleText = UdonArrayPlus.StringsAdd(_subtitleText, text);
                            _subtitleStartTime = UdonArrayPlus.FloatsAdd(_subtitleStartTime, startTime);
                            _subtitleEndTime = UdonArrayPlus.FloatsAdd(_subtitleEndTime, endTime);
                            startTime = -1f;
                            endTime = -1f;
                            text = "";
                        }
                        i++;
                    }
                    else
                    {
                        // if (text != "" && line != "")
                        if (text != "")
                            text += "\n";
                        text += line;
                    }
                }
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Contains(" --> ")
                    && char.IsDigit(line[0]) && char.IsDigit(line[line.Length - 1]))
                    {
                        string[] times = line.Split(new string[] { " --> " }, System.StringSplitOptions.None);
                        if (times.Length == 2)
                        {
                            startTime = ParseTime(times[0]);
                            endTime = ParseTime(times[1]);
                        }
                    }
                    else if (line == ""
                    && i + 1 < lines.Length
                    && lines[i + 1].Contains(" --> ")
                    && char.IsDigit(lines[i + 1][0]) && char.IsDigit(lines[i + 1][line.Length - 1]))
                    {
                        if (startTime >= 0f && endTime >= 0f && text != "")
                        {
                            _subtitleText = UdonArrayPlus.StringsAdd(_subtitleText, text);
                            _subtitleStartTime = UdonArrayPlus.FloatsAdd(_subtitleStartTime, startTime);
                            _subtitleEndTime = UdonArrayPlus.FloatsAdd(_subtitleEndTime, endTime);
                            startTime = -1f;
                            endTime = -1f;
                            text = "";
                        }
                    }
                    else
                    {
                        if (text != "")
                            text += "\n";
                        text += line;
                    }
                }
            }
            if (startTime >= 0f && endTime >= 0f && text != "")
            {
                _subtitleText = UdonArrayPlus.StringsAdd(_subtitleText, text);
                _subtitleStartTime = UdonArrayPlus.FloatsAdd(_subtitleStartTime, startTime);
                _subtitleEndTime = UdonArrayPlus.FloatsAdd(_subtitleEndTime, endTime);
            }
            Debug.Log($"name: {subtitle.name}, text Length: {_subtitleText.Length}");
            subtitle.subtitleText = _subtitleText;
            subtitle.subtitleStartTime = _subtitleStartTime;
            subtitle.subtitleEndTime = _subtitleEndTime;
        }
        float ParseTime(string time)
        {
            // 验证第一位和最后一位是否为数字
            if (!char.IsDigit(time[0]) || !char.IsDigit(time[time.Length - 1]))
                return -1f;
            // 00:00:31,660
            // 00:00:34,160
            string[] times = time.Trim().Split(new string[] { ":", "," }, System.StringSplitOptions.None);
            if (times.Length == 4)
            {
                float hour = float.Parse(times[0]);
                float minute = float.Parse(times[1]);
                float second = float.Parse(times[2]);
                float millisecond = float.Parse(times[3]);
                return hour * 3600f + minute * 60f + second + millisecond / 1000f;
            }
            return -1f;
        }
    }
}
