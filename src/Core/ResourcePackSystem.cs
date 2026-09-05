using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class ResourcePackSystem
    {
        private readonly List<ResourcePack> loadedPacks;
        private readonly Dictionary<string, string> textureOverrides;
        private readonly Dictionary<string, string> soundOverrides;
        private readonly Dictionary<string, string> langOverrides;
        private readonly string resourcePacksDirectory;

        // 事件
        public event Action<ResourcePack> OnPackLoaded;
        public event Action<ResourcePack> OnPackUnloaded;
        public event Action OnPacksChanged;

        public int LoadedPackCount => loadedPacks.Count;
        public List<ResourcePack> LoadedPacks => new List<ResourcePack>(loadedPacks);

        public ResourcePackSystem(string resourcePacksDirectory)
        {
            this.resourcePacksDirectory = resourcePacksDirectory;
            loadedPacks = new List<ResourcePack>();
            textureOverrides = new Dictionary<string, string>();
            soundOverrides = new Dictionary<string, string>();
            langOverrides = new Dictionary<string, string>();
        }

        public void Initialize()
        {
            Console.WriteLine("[ResourcePackSystem] 资源包系统初始化完成");
            Console.WriteLine($"[ResourcePackSystem] 资源包目录: {resourcePacksDirectory}");

            if (!Directory.Exists(resourcePacksDirectory))
            {
                Directory.CreateDirectory(resourcePacksDirectory);
                Console.WriteLine("[ResourcePackSystem] 已创建资源包目录");
            }

            // 加载默认资源包
            LoadDefaultResources();
        }

        private void LoadDefaultResources()
        {
            ResourcePack defaultPack = new ResourcePack
            {
                Name = "默认资源包",
                Description = "VoxelCraft 默认资源包",
                Version = "1.0.0",
                Author = "VoxelCraft Team",
                Directory = "default",
                IsEnabled = true,
                IsDefault = true,
                Priority = 0
            };

            loadedPacks.Add(defaultPack);
            Console.WriteLine("[ResourcePackSystem] 已加载默认资源包");
        }

        public bool LoadPack(string packPath)
        {
            if (!Directory.Exists(packPath) && !File.Exists(packPath))
            {
                Console.WriteLine($"[ResourcePackSystem] 资源包不存在: {packPath}");
                return false;
            }

            string packName = Path.GetFileNameWithoutExtension(packPath);

            // 检查是否已加载
            if (loadedPacks.Exists(p => p.Name == packName))
            {
                Console.WriteLine($"[ResourcePackSystem] 资源包已加载: {packName}");
                return false;
            }

            try
            {
                ResourcePack pack = new ResourcePack
                {
                    Name = packName,
                    Directory = packPath,
                    IsEnabled = true,
                    IsDefault = false,
                    Priority = loadedPacks.Count
                };

                // 读取pack.mcmeta
                string metaPath = Path.Combine(packPath, "pack.mcmeta");
                if (File.Exists(metaPath))
                {
                    ParsePackMeta(pack, metaPath);
                }

                // 加载资源
                LoadPackResources(pack);

                loadedPacks.Add(pack);
                OnPackLoaded?.Invoke(pack);
                OnPacksChanged?.Invoke();

                Console.WriteLine($"[ResourcePackSystem] 已加载资源包: {pack.Name} v{pack.Version} by {pack.Author}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourcePackSystem] 加载资源包失败: {packName} - {ex.Message}");
                return false;
            }
        }

        public bool UnloadPack(string packName)
        {
            ResourcePack pack = loadedPacks.Find(p => p.Name == packName);
            if (pack == null)
            {
                Console.WriteLine($"[ResourcePackSystem] 未找到资源包: {packName}");
                return false;
            }

            if (pack.IsDefault)
            {
                Console.WriteLine("[ResourcePackSystem] 不能卸载默认资源包");
                return false;
            }

            try
            {
                // 移除资源覆盖
                UnloadPackResources(pack);

                loadedPacks.Remove(pack);
                OnPackUnloaded?.Invoke(pack);
                OnPacksChanged?.Invoke();

                Console.WriteLine($"[ResourcePackSystem] 已卸载资源包: {pack.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourcePackSystem] 卸载资源包失败: {packName} - {ex.Message}");
                return false;
            }
        }

        public void EnablePack(string packName)
        {
            ResourcePack pack = loadedPacks.Find(p => p.Name == packName);
            if (pack != null)
            {
                pack.IsEnabled = true;
                OnPacksChanged?.Invoke();
            }
        }

        public void DisablePack(string packName)
        {
            ResourcePack pack = loadedPacks.Find(p => p.Name == packName);
            if (pack != null && !pack.IsDefault)
            {
                pack.IsEnabled = false;
                OnPacksChanged?.Invoke();
            }
        }

        public void SetPackPriority(string packName, int priority)
        {
            ResourcePack pack = loadedPacks.Find(p => p.Name == packName);
            if (pack != null)
            {
                pack.Priority = priority;
                loadedPacks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                OnPacksChanged?.Invoke();
            }
        }

        public void MovePackUp(string packName)
        {
            int index = loadedPacks.FindIndex(p => p.Name == packName);
            if (index > 1) // 0是默认包
            {
                ResourcePack temp = loadedPacks[index];
                loadedPacks[index] = loadedPacks[index - 1];
                loadedPacks[index - 1] = temp;

                // 更新优先级
                for (int i = 0; i < loadedPacks.Count; i++)
                {
                    loadedPacks[i].Priority = i;
                }

                OnPacksChanged?.Invoke();
            }
        }

        public void MovePackDown(string packName)
        {
            int index = loadedPacks.FindIndex(p => p.Name == packName);
            if (index >= 0 && index < loadedPacks.Count - 1)
            {
                ResourcePack temp = loadedPacks[index];
                loadedPacks[index] = loadedPacks[index + 1];
                loadedPacks[index + 1] = temp;

                // 更新优先级
                for (int i = 0; i < loadedPacks.Count; i++)
                {
                    loadedPacks[i].Priority = i;
                }

                OnPacksChanged?.Invoke();
            }
        }

        private void ParsePackMeta(ResourcePack pack, string metaPath)
        {
            try
            {
                string[] lines = File.ReadAllLines(metaPath);
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("\"description\""))
                    {
                        int start = trimmed.IndexOf('"') + 1;
                        int end = trimmed.LastIndexOf('"');
                        if (end > start)
                        {
                            pack.Description = trimmed.Substring(start, end - start);
                        }
                    }
                    else if (trimmed.StartsWith("\"pack_format\""))
                    {
                        string value = new string(trimmed.Where(char.IsDigit).ToArray());
                        if (!string.IsNullOrEmpty(value))
                        {
                            pack.PackFormat = int.Parse(value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourcePackSystem] 解析pack.mcmeta失败: {ex.Message}");
            }
        }

        private void LoadPackResources(ResourcePack pack)
        {
            string assetsPath = Path.Combine(pack.Directory, "assets");
            if (!Directory.Exists(assetsPath)) return;

            // 加载纹理覆盖
            string texturesPath = Path.Combine(assetsPath, "textures");
            if (Directory.Exists(texturesPath))
            {
                LoadTextureOverrides(texturesPath, "");
            }

            // 加载音效覆盖
            string soundsPath = Path.Combine(assetsPath, "sounds");
            if (Directory.Exists(soundsPath))
            {
                LoadSoundOverrides(soundsPath, "");
            }

            // 加载语言覆盖
            string langPath = Path.Combine(assetsPath, "lang");
            if (Directory.Exists(langPath))
            {
                LoadLangOverrides(langPath);
            }
        }

        private void LoadTextureOverrides(string basePath, string relativePath)
        {
            string currentPath = Path.Combine(basePath, relativePath);

            foreach (string file in Directory.GetFiles(currentPath, "*.png"))
            {
                string fileName = Path.GetFileName(file);
                string key = string.IsNullOrEmpty(relativePath) ? fileName : $"{relativePath}/{fileName}";
                textureOverrides[key] = file;
            }

            foreach (string dir in Directory.GetDirectories(currentPath))
            {
                string dirName = Path.GetFileName(dir);
                string newRelative = string.IsNullOrEmpty(relativePath) ? dirName : $"{relativePath}/{dirName}";
                LoadTextureOverrides(basePath, newRelative);
            }
        }

        private void LoadSoundOverrides(string basePath, string relativePath)
        {
            string currentPath = Path.Combine(basePath, relativePath);

            foreach (string file in Directory.GetFiles(currentPath, "*.ogg"))
            {
                string fileName = Path.GetFileName(file);
                string key = string.IsNullOrEmpty(relativePath) ? fileName : $"{relativePath}/{fileName}";
                soundOverrides[key] = file;
            }

            foreach (string dir in Directory.GetDirectories(currentPath))
            {
                string dirName = Path.GetFileName(dir);
                string newRelative = string.IsNullOrEmpty(relativePath) ? dirName : $"{relativePath}/{dirName}";
                LoadSoundOverrides(basePath, newRelative);
            }
        }

        private void LoadLangOverrides(string langPath)
        {
            foreach (string file in Directory.GetFiles(langPath, "*.json"))
            {
                string langCode = Path.GetFileNameWithoutExtension(file);
                langOverrides[langCode] = file;
            }
        }

        private void UnloadPackResources(ResourcePack pack)
        {
            // 移除该包的资源覆盖
            // 简化实现：重新加载所有启用的包
            textureOverrides.Clear();
            soundOverrides.Clear();
            langOverrides.Clear();

            foreach (ResourcePack p in loadedPacks)
            {
                if (p.IsEnabled && p != pack)
                {
                    LoadPackResources(p);
                }
            }
        }

        public string GetTexturePath(string textureKey)
        {
            if (textureOverrides.TryGetValue(textureKey, out string path))
            {
                return path;
            }
            return null;
        }

        public string GetSoundPath(string soundKey)
        {
            if (soundOverrides.TryGetValue(soundKey, out string path))
            {
                return path;
            }
            return null;
        }

        public string GetLangFilePath(string langCode)
        {
            if (langOverrides.TryGetValue(langCode, out string path))
            {
                return path;
            }
            return null;
        }

        public List<string> GetAvailablePacks()
        {
            if (!Directory.Exists(resourcePacksDirectory))
            {
                return new List<string>();
            }

            List<string> result = new List<string>();

            foreach (string dir in Directory.GetDirectories(resourcePacksDirectory))
            {
                result.Add(Path.GetFileName(dir));
            }

            foreach (string file in Directory.GetFiles(resourcePacksDirectory, "*.zip"))
            {
                result.Add(Path.GetFileName(file));
            }

            return result;
        }

        public void ReloadAllPacks()
        {
            Console.WriteLine("[ResourcePackSystem] 重新加载所有资源包...");

            textureOverrides.Clear();
            soundOverrides.Clear();
            langOverrides.Clear();

            foreach (ResourcePack pack in loadedPacks)
            {
                if (pack.IsEnabled && !pack.IsDefault)
                {
                    LoadPackResources(pack);
                }
            }

            OnPacksChanged?.Invoke();
            Console.WriteLine("[ResourcePackSystem] 资源包重新加载完成");
        }

        public string GetResourcePacksDirectory()
        {
            return resourcePacksDirectory;
        }
    }

    public class ResourcePack
    {
        public string Name;
        public string Description;
        public string Version;
        public string Author;
        public string Directory;
        public bool IsEnabled;
        public bool IsDefault;
        public int Priority;
        public int PackFormat;

        public override string ToString()
        {
            return $"{Name} v{Version}";
        }
    }
}
