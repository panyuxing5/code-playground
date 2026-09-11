using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Items;
using VoxelCraft.Entities;

namespace VoxelCraft.Core
{
    public class ModLoader
    {
        private readonly List<ModInfo> loadedMods;
        private readonly Dictionary<string, Assembly> modAssemblies;
        private readonly string modsDirectory;

        // 事件
        public event Action<ModInfo> OnModLoaded;
        public event Action<ModInfo> OnModUnloaded;
        public event Action<string, Exception> OnModError;

        public int LoadedModCount => loadedMods.Count;
        public List<ModInfo> LoadedMods => new List<ModInfo>(loadedMods);

        public ModLoader(string modsDirectory)
        {
            this.modsDirectory = modsDirectory;
            loadedMods = new List<ModInfo>();
            modAssemblies = new Dictionary<string, Assembly>();
        }

        public void Initialize()
        {
            Console.WriteLine("[ModLoader] 模组加载器初始化完成");
            Console.WriteLine($"[ModLoader] 模组目录: {modsDirectory}");

            if (!Directory.Exists(modsDirectory))
            {
                Directory.CreateDirectory(modsDirectory);
                Console.WriteLine("[ModLoader] 已创建模组目录");
            }
        }

        public void LoadAllMods()
        {
            Console.WriteLine("[ModLoader] 开始加载模组...");

            if (!Directory.Exists(modsDirectory))
            {
                Console.WriteLine("[ModLoader] 模组目录不存在，跳过加载");
                return;
            }

            string[] modFiles = Directory.GetFiles(modsDirectory, "*.dll");

            foreach (string modFile in modFiles)
            {
                try
                {
                    LoadMod(modFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ModLoader] 加载模组失败: {Path.GetFileName(modFile)} - {ex.Message}");
                    OnModError?.Invoke(Path.GetFileName(modFile), ex);
                }
            }

            Console.WriteLine($"[ModLoader] 加载完成，共加载 {loadedMods.Count} 个模组");
        }

        public bool LoadMod(string modFilePath)
        {
            if (!File.Exists(modFilePath))
            {
                Console.WriteLine($"[ModLoader] 模组文件不存在: {modFilePath}");
                return false;
            }

            string modId = Path.GetFileNameWithoutExtension(modFilePath);

            if (modAssemblies.ContainsKey(modId))
            {
                Console.WriteLine($"[ModLoader] 模组已加载: {modId}");
                return false;
            }

            try
            {
                // 加载程序集
                Assembly assembly = Assembly.LoadFrom(modFilePath);
                modAssemblies[modId] = assembly;

                // 查找模组主类
                Type modType = FindModMainClass(assembly);
                if (modType == null)
                {
                    Console.WriteLine($"[ModLoader] 未找到模组主类: {modId}");
                    modAssemblies.Remove(modId);
                    return false;
                }

                // 创建模组实例
                object modInstance = Activator.CreateInstance(modType);

                // 获取模组信息
                ModInfo modInfo = GetModInfo(modInstance, modId);
                modInfo.Assembly = assembly;
                modInfo.Instance = modInstance;
                modInfo.FilePath = modFilePath;

                // 调用初始化方法
                MethodInfo initMethod = modType.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Instance);
                if (initMethod != null)
                {
                    initMethod.Invoke(modInstance, null);
                }

                loadedMods.Add(modInfo);
                OnModLoaded?.Invoke(modInfo);

                Console.WriteLine($"[ModLoader] 已加载模组: {modInfo.Name} v{modInfo.Version} by {modInfo.Author}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModLoader] 加载模组异常: {modId} - {ex.Message}");
                OnModError?.Invoke(modId, ex);
                return false;
            }
        }

        public bool UnloadMod(string modId)
        {
            ModInfo modInfo = loadedMods.Find(m => m.Id == modId);
            if (modInfo == null)
            {
                Console.WriteLine($"[ModLoader] 未找到模组: {modId}");
                return false;
            }

            try
            {
                // 调用卸载方法
                if (modInfo.Instance != null)
                {
                    MethodInfo shutdownMethod = modInfo.Instance.GetType().GetMethod("Shutdown", BindingFlags.Public | BindingFlags.Instance);
                    if (shutdownMethod != null)
                    {
                        shutdownMethod.Invoke(modInfo.Instance, null);
                    }
                }

                loadedMods.Remove(modInfo);
                modAssemblies.Remove(modId);
                OnModUnloaded?.Invoke(modInfo);

                Console.WriteLine($"[ModLoader] 已卸载模组: {modInfo.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModLoader] 卸载模组异常: {modId} - {ex.Message}");
                return false;
            }
        }

        public void UnloadAllMods()
        {
            Console.WriteLine("[ModLoader] 开始卸载所有模组...");

            for (int i = loadedMods.Count - 1; i >= 0; i--)
            {
                UnloadMod(loadedMods[i].Id);
            }

            Console.WriteLine("[ModLoader] 所有模组已卸载");
        }

        private Type FindModMainClass(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // 检查是否有 Mod 特性
                object[] attributes = type.GetCustomAttributes(typeof(ModAttribute), false);
                if (attributes.Length > 0)
                {
                    return type;
                }

                // 检查是否实现了 IMod 接口
                if (typeof(IMod).IsAssignableFrom(type))
                {
                    return type;
                }
            }

            return null;
        }

        private ModInfo GetModInfo(object modInstance, string defaultId)
        {
            ModInfo info = new ModInfo
            {
                Id = defaultId,
                Name = defaultId,
                Version = "1.0.0",
                Author = "Unknown",
                Description = "",
                Website = "",
                Dependencies = new List<string>()
            };

            Type type = modInstance.GetType();

            // 从特性获取信息
            object[] attributes = type.GetCustomAttributes(typeof(ModAttribute), false);
            if (attributes.Length > 0)
            {
                ModAttribute modAttr = (ModAttribute)attributes[0];
                info.Id = modAttr.Id ?? defaultId;
                info.Name = modAttr.Name ?? defaultId;
                info.Version = modAttr.Version ?? "1.0.0";
                info.Author = modAttr.Author ?? "Unknown";
                info.Description = modAttr.Description ?? "";
                info.Website = modAttr.Website ?? "";
                if (modAttr.Dependencies != null)
                {
                    info.Dependencies = new List<string>(modAttr.Dependencies);
                }
            }

            // 从属性获取信息
            PropertyInfo idProp = type.GetProperty("Id");
            if (idProp != null) info.Id = idProp.GetValue(modInstance)?.ToString() ?? info.Id;

            PropertyInfo nameProp = type.GetProperty("Name");
            if (nameProp != null) info.Name = nameProp.GetValue(modInstance)?.ToString() ?? info.Name;

            PropertyInfo versionProp = type.GetProperty("Version");
            if (versionProp != null) info.Version = versionProp.GetValue(modInstance)?.ToString() ?? info.Version;

            PropertyInfo authorProp = type.GetProperty("Author");
            if (authorProp != null) info.Author = authorProp.GetValue(modInstance)?.ToString() ?? info.Author;

            return info;
        }

        public ModInfo GetModInfo(string modId)
        {
            return loadedMods.Find(m => m.Id == modId);
        }

        public bool IsModLoaded(string modId)
        {
            return loadedMods.Exists(m => m.Id == modId);
        }

        public T GetModInstance<T>(string modId) where T : class
        {
            ModInfo modInfo = loadedMods.Find(m => m.Id == modId);
            if (modInfo != null && modInfo.Instance is T typedInstance)
            {
                return typedInstance;
            }
            return null;
        }

        public void UpdateMods()
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo updateMethod = modInfo.Instance.GetType().GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
                if (updateMethod != null)
                {
                    try
                    {
                        updateMethod.Invoke(modInfo.Instance, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组更新异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void RenderMods()
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo renderMethod = modInfo.Instance.GetType().GetMethod("Render", BindingFlags.Public | BindingFlags.Instance);
                if (renderMethod != null)
                {
                    try
                    {
                        renderMethod.Invoke(modInfo.Instance, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组渲染异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnBlockPlaced(int x, int y, int z, ushort blockId)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnBlockPlaced", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { x, y, z, blockId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnBlockBroken(int x, int y, int z, ushort blockId)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnBlockBroken", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { x, y, z, blockId });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnEntitySpawned(Entity entity)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnEntitySpawned", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { entity });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnEntityDied(Entity entity)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnEntityDied", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { entity });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnPlayerJoined(string playerName)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnPlayerJoined", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { playerName });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnPlayerLeft(string playerName)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnPlayerLeft", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { playerName });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public void OnCommandExecuted(string command, string[] args)
        {
            foreach (ModInfo modInfo in loadedMods)
            {
                if (modInfo.Instance == null) continue;

                MethodInfo method = modInfo.Instance.GetType().GetMethod("OnCommandExecuted", BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    try
                    {
                        method.Invoke(modInfo.Instance, new object[] { command, args });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModLoader] 模组事件异常: {modInfo.Id} - {ex.Message}");
                    }
                }
            }
        }

        public List<string> GetAvailableModFiles()
        {
            if (!Directory.Exists(modsDirectory))
            {
                return new List<string>();
            }

            string[] files = Directory.GetFiles(modsDirectory, "*.dll");
            List<string> result = new List<string>();
            foreach (string file in files)
            {
                result.Add(Path.GetFileName(file));
            }
            return result;
        }

        public string GetModsDirectory()
        {
            return modsDirectory;
        }
    }

    public class ModInfo
    {
        public string Id;
        public string Name;
        public string Version;
        public string Author;
        public string Description;
        public string Website;
        public List<string> Dependencies;
        public Assembly Assembly;
        public object Instance;
        public string FilePath;
        public bool IsEnabled = true;

        public override string ToString()
        {
            return $"{Name} v{Version} by {Author}";
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModAttribute : Attribute
    {
        public string Id;
        public string Name;
        public string Version;
        public string Author;
        public string Description;
        public string Website;
        public string[] Dependencies;

        public ModAttribute(string id)
        {
            Id = id;
        }
    }

    public interface IMod
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        string Author { get; }

        void Initialize();
        void Shutdown();
        void Update();
        void Render();
    }
}
