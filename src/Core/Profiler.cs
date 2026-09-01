using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class Profiler
    {
        private static Profiler instance;
        public static Profiler Instance => instance ??= new Profiler();

        // 性能计数器
        private readonly Dictionary<string, PerformanceCounter> counters;
        private readonly Dictionary<string, long> startTimes;
        private readonly Dictionary<string, int> callCounts;

        // 帧统计
        public float FrameTime { get; private set; }
        public float FPS { get; private set; }
        public float DeltaTime { get; private set; }
        public int FrameCount { get; private set; }

        // 内存统计
        public long ManagedMemory { get; private set; }
        public long TotalMemory { get; private set; }
        public int CollectionCount { get; private set; }

        // 线程统计
        public int ActiveThreads { get; private set; }
        public int ThreadPoolThreads { get; private set; }

        // 游戏特定统计
        public int ChunksLoaded { get; set; }
        public int ChunksRendered { get; set; }
        public int FacesRendered { get; set; }
        public int EntitiesCount { get; set; }
        public int ParticlesCount { get; set; }

        // 历史数据
        private readonly Queue<float> frameTimeHistory;
        private const int HistorySize = 60;

        // 停止watch
        private readonly Stopwatch frameStopwatch;
        private float fpsAccumulator;
        private int fpsFrameCount;

        private Profiler()
        {
            counters = new Dictionary<string, PerformanceCounter>();
            startTimes = new Dictionary<string, long>();
            callCounts = new Dictionary<string, int>();
            frameTimeHistory = new Queue<float>(HistorySize);
            frameStopwatch = new Stopwatch();
            frameStopwatch.Start();
        }

        public void BeginFrame()
        {
            frameStopwatch.Restart();
            FrameCount++;
        }

        public void EndFrame()
        {
            frameStopwatch.Stop();
            FrameTime = (float)frameStopwatch.Elapsed.TotalMilliseconds;
            DeltaTime = FrameTime / 1000.0f;

            // FPS计算
            fpsAccumulator += DeltaTime;
            fpsFrameCount++;
            if (fpsAccumulator >= 1.0f)
            {
                FPS = fpsFrameCount / fpsAccumulator;
                fpsAccumulator = 0;
                fpsFrameCount = 0;
            }

            // 历史记录
            frameTimeHistory.Enqueue(FrameTime);
            if (frameTimeHistory.Count > HistorySize)
            {
                frameTimeHistory.Dequeue();
            }

            // 内存统计
            ManagedMemory = GC.GetTotalMemory(false);
            CollectionCount = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2);

            // 线程统计
            ActiveThreads = Process.GetCurrentProcess().Threads.Count;
            ThreadPoolThreads = ThreadPool.ThreadCount;
        }

        public void StartSection(string name)
        {
            long timestamp = Stopwatch.GetTimestamp();
            startTimes[name] = timestamp;

            if (!callCounts.ContainsKey(name))
            {
                callCounts[name] = 0;
            }
            callCounts[name]++;
        }

        public void EndSection(string name)
        {
            if (!startTimes.TryGetValue(name, out long startTime)) return;

            long endTime = Stopwatch.GetTimestamp();
            double elapsedMs = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;

            if (!counters.TryGetValue(name, out PerformanceCounter counter))
            {
                counter = new PerformanceCounter();
                counters[name] = counter;
            }

            counter.AddSample(elapsedMs);
            startTimes.Remove(name);
        }

        public PerformanceCounter GetCounter(string name)
        {
            counters.TryGetValue(name, out PerformanceCounter counter);
            return counter;
        }

        public Dictionary<string, PerformanceCounter> GetAllCounters()
        {
            return new Dictionary<string, PerformanceCounter>(counters);
        }

        public void ResetCounters()
        {
            counters.Clear();
            callCounts.Clear();
        }

        public float GetAverageFrameTime()
        {
            if (frameTimeHistory.Count == 0) return 0;

            float sum = 0;
            foreach (float time in frameTimeHistory)
            {
                sum += time;
            }
            return sum / frameTimeHistory.Count;
        }

        public float GetMinFrameTime()
        {
            if (frameTimeHistory.Count == 0) return 0;

            float min = float.MaxValue;
            foreach (float time in frameTimeHistory)
            {
                if (time < min) min = time;
            }
            return min;
        }

        public float GetMaxFrameTime()
        {
            if (frameTimeHistory.Count == 0) return 0;

            float max = float.MinValue;
            foreach (float time in frameTimeHistory)
            {
                if (time > max) max = time;
            }
            return max;
        }

        public float Get99thPercentileFrameTime()
        {
            if (frameTimeHistory.Count == 0) return 0;

            List<float> times = new List<float>(frameTimeHistory);
            times.Sort();
            int index = (int)(times.Count * 0.99);
            return times[Math.Min(index, times.Count - 1)];
        }

        public string GetPerformanceReport()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("=== 性能报告 ===");
            sb.AppendLine($"FPS: {FPS:F1}");
            sb.AppendLine($"帧时间: {FrameTime:F2}ms");
            sb.AppendLine($"平均帧时间: {GetAverageFrameTime():F2}ms");
            sb.AppendLine($"最小帧时间: {GetMinFrameTime():F2}ms");
            sb.AppendLine($"最大帧时间: {GetMaxFrameTime():F2}ms");
            sb.AppendLine($"99%帧时间: {Get99thPercentileFrameTime():F2}ms");
            sb.AppendLine();
            sb.AppendLine($"内存: {ManagedMemory / 1024 / 1024:F1} MB");
            sb.AppendLine($"GC收集次数: {CollectionCount}");
            sb.AppendLine($"活跃线程: {ActiveThreads}");
            sb.AppendLine();
            sb.AppendLine($"已加载区块: {ChunksLoaded}");
            sb.AppendLine($"已渲染区块: {ChunksRendered}");
            sb.AppendLine($"已渲染面: {FacesRendered}");
            sb.AppendLine($"实体数量: {EntitiesCount}");
            sb.AppendLine($"粒子数量: {ParticlesCount}");
            sb.AppendLine();
            sb.AppendLine("=== 性能计数器 ===");

            foreach (var kvp in counters)
            {
                sb.AppendLine($"{kvp.Key}:");
                sb.AppendLine($"  平均: {kvp.Value.Average:F4}ms");
                sb.AppendLine($"  最小: {kvp.Value.Min:F4}ms");
                sb.AppendLine($"  最大: {kvp.Value.Max:F4}ms");
                sb.AppendLine($"  调用次数: {kvp.Value.SampleCount}");
            }

            return sb.ToString();
        }

        public void LogPerformanceReport()
        {
            Console.WriteLine(GetPerformanceReport());
        }
    }

    public class PerformanceCounter
    {
        public double Total { get; private set; }
        public double Min { get; private set; } = double.MaxValue;
        public double Max { get; private set; } = double.MinValue;
        public int SampleCount { get; private set; }

        public double Average => SampleCount > 0 ? Total / SampleCount : 0;

        public void AddSample(double value)
        {
            Total += value;
            Min = Math.Min(Min, value);
            Max = Math.Max(Max, value);
            SampleCount++;
        }

        public void Reset()
        {
            Total = 0;
            Min = double.MaxValue;
            Max = double.MinValue;
            SampleCount = 0;
        }
    }
}
