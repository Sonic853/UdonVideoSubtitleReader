
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.VideoSubtitle
{
    public class VideoSubtitle : UdonSharpBehaviour
    {
        /// <summary>
        /// 字幕文件
        /// </summary>
        [Header("字幕文件")]
        public TextAsset subtitleFile;
        /// <summary>
        /// 字幕文本
        /// </summary>
        [HideInInspector]
        // [SerializeField]
        // [TextArea(3, 10)]
        public string[] subtitleText = new string[0];
        /// <summary>
        /// 字幕开始时间
        /// </summary>
        [HideInInspector]
        // [SerializeField]
        public float[] subtitleStartTime = new float[0];
        /// <summary>
        /// 字幕结束时间
        /// </summary>
        [HideInInspector]
        // [SerializeField]
        public float[] subtitleEndTime = new float[0];
        /// <summary>
        /// 偏移
        /// </summary>
        [Header("偏移")]
        public float offset = 0f;
    }
}
