using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

namespace AbcConsole.Internal
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private Text text = default;

        private readonly List<FrameData> _frames = new List<FrameData>();

        public void OnEnable()
        {
            _frames.Clear();
            text.text = "";
        }

        public void Update()
        {
            var targetFrameRate = Application.targetFrameRate;
            if (targetFrameRate <= 0) targetFrameRate = 30;

            _frames.Add(Get());
            while (_frames.Count > targetFrameRate)
            {
                _frames.RemoveAt(0);
            }

            if (Time.frameCount % targetFrameRate == 0)
            {
                var content = new StringBuilder();
                var framesCount = _frames.Count;

                {
                    var total = 0f;
                    for (var i = 0; i < framesCount; ++i) total += _frames[i].DeltaTime;
                    content.Append("FPS: ");
                    content.AppendFormat("{0:0.00}", 1f / (total / framesCount));
                }

                content.Append("\n");

                content.Append("Memory:");
                content.Append("\n");

                {
                    var total = 0f;
                    for (var i = 0; i < framesCount; ++i) total += _frames[i].TotalReservedMemory;
                    content.Append("  Reserved: ");
                    content.AppendFormat("{0:0}MB", total / framesCount);
                }

                content.Append("\n");

                {
                    var total = 0f;
                    for (var i = 0; i < framesCount; ++i) total += _frames[i].TotalAllocatedMemory;
                    content.Append("  Allocated: ");
                    content.AppendFormat("{0:0}MB", total / framesCount);
                }

                content.Append("\n");

                {
                    var total = 0f;
                    for (var i = 0; i < framesCount; ++i) total += _frames[i].AllocatedMemoryForGraphicsDriver;
                    content.Append("  Allocated(GPU): ");
                    content.AppendFormat("{0:0}MB", total / framesCount);
                }

                text.text = content.ToString();
            }
        }

        private FrameData Get()
        {
            return new FrameData
            {
                DeltaTime = Time.unscaledDeltaTime,
                TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f,
                TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f,
                AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f,
            };
        }
    }

    public struct FrameData
    {
        public float DeltaTime { get; set; }
        public float TotalReservedMemory { get; set; }
        public float TotalAllocatedMemory { get; set; }
        public float AllocatedMemoryForGraphicsDriver { get; set; }
    }
}
