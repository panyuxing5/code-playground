using System;
using System.IO;
using VoxelCraft.Core;

namespace VoxelCraft
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  VoxelCraft - 3D体素沙盒游戏");
            Console.WriteLine("  版本: 1.0.0");
            Console.WriteLine("========================================");
            Console.WriteLine();

            try
            {
                using (var game = new GameEngine())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"游戏崩溃: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                File.WriteAllText("crash_log.txt", ex.ToString());
                Console.WriteLine("错误日志已保存到 crash_log.txt");
                Console.ReadKey();
            }
        }
    }
}
