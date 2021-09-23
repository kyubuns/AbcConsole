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

                var cpuTimeTotal = 0.0;
                for (var i = 0; i < framesCount; ++i) cpuTimeTotal += _frames[i].CpuTime;
                if (cpuTimeTotal > 0.01)
                {
                    content.Append("CPU: ");
                    content.AppendFormat("{0:0}ms", cpuTimeTotal / framesCount);
                    content.Append("\n");
                }

                var gpuTimeTotal = 0.0;
                for (var i = 0; i < framesCount; ++i) gpuTimeTotal += _frames[i].GpuTime;
                if (gpuTimeTotal > 0.01)
                {
                    content.Append("GPU: ");
                    content.AppendFormat("{0:0}ms", gpuTimeTotal / framesCount);
                    content.Append("\n");
                }

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

        private readonly FrameTiming[] _frameTimings = new FrameTiming[1];
        private FrameData Get()
        {
            FrameTimingManager.CaptureFrameTimings();
            var numFrames = FrameTimingManager.GetLatestTimings((uint) _frameTimings.Length, _frameTimings);
            if (numFrames == 0) _frameTimings[0] = new FrameTiming();

            return new FrameData
            {
                DeltaTime = Time.unscaledDeltaTime,
                CpuTime = _frameTimings[0].cpuFrameTime,
                GpuTime = _frameTimings[0].gpuFrameTime,
                TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f,
                TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f,
                AllocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024f / 1024f,
            };
        }
    }

    public struct FrameData
    {
        public float DeltaTime { get; set; }
        public double CpuTime { get; set; }
        public double GpuTime { get; set; }
        public float TotalReservedMemory { get; set; }
        public float TotalAllocatedMemory { get; set; }
        public float AllocatedMemoryForGraphicsDriver { get; set; }
    }
}
