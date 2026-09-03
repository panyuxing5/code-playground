using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class SkinSystem
    {
        private readonly Dictionary<string, SkinData> skins;
        private readonly string skinsDirectory;
        private SkinData currentSkin;
        private string currentSkinName;

        // 事件
        public event Action<SkinData> OnSkinChanged;
        public event Action<string> OnSkinLoaded;

        public int SkinCount => skins.Count;
        public SkinData CurrentSkin => currentSkin;
        public string CurrentSkinName => currentSkinName;

        public SkinSystem(string skinsDirectory)
        {
            this.skinsDirectory = skinsDirectory;
            skins = new Dictionary<string, SkinData>();
        }

        public void Initialize()
        {
            Console.WriteLine("[SkinSystem] 皮肤系统初始化完成");
            Console.WriteLine($"[SkinSystem] 皮肤目录: {skinsDirectory}");

            if (!Directory.Exists(skinsDirectory))
            {
                Directory.CreateDirectory(skinsDirectory);
                Console.WriteLine("[SkinSystem] 已创建皮肤目录");
            }

            // 加载默认皮肤
            LoadDefaultSkins();

            // 加载自定义皮肤
            LoadCustomSkins();
        }

        private void LoadDefaultSkins()
        {
            // Steve 皮肤
            SkinData steve = new SkinData
            {
                Name = "Steve",
                Author = "Mojang",
                Description = "默认 Steve 皮肤",
                SkinType = SkinType.Classic,
                IsCustom = false,
                IsSelected = true
            };
            skins["steve"] = steve;

            // Alex 皮肤
            SkinData alex = new SkinData
            {
                Name = "Alex",
                Author = "Mojang",
                Description = "默认 Alex 皮肤（细手臂）",
                SkinType = SkinType.Slim,
                IsCustom = false,
                IsSelected = false
            };
            skins["alex"] = alex;

            // Noor 皮肤
            SkinData noor = new SkinData
            {
                Name = "Noor",
                Author = "Mojang",
                Description = "默认 Noor 皮肤",
                SkinType = SkinType.Classic,
                IsCustom = false,
                IsSelected = false
            };
            skins["noor"] = noor;

            // Zuri 皮肤
            SkinData zuri = new SkinData
            {
                Name = "Zuri",
                Author = "Mojang",
                Description = "默认 Zuri 皮肤（细手臂）",
                SkinType = SkinType.Slim,
                IsCustom = false,
                IsSelected = false
            };
            skins["zuri"] = zuri;

            // Sunny 皮肤
            SkinData sunny = new SkinData
            {
                Name = "Sunny",
                Author = "Mojang",
                Description = "默认 Sunny 皮肤",
                SkinType = SkinType.Classic,
                IsCustom = false,
                IsSelected = false
            };
            skins["sunny"] = sunny;

            // Ari 皮肤
            SkinData ari = new SkinData
            {
                Name = "Ari",
                Author = "Mojang",
                Description = "默认 Ari 皮肤（细手臂）",
                SkinType = SkinType.Slim,
                IsCustom = false,
                IsSelected = false
            };
            skins["ari"] = ari;

            // Efe 皮肤
            SkinData efe = new SkinData
            {
                Name = "Efe",
                Author = "Mojang",
                Description = "默认 Efe 皮肤",
                SkinType = SkinType.Classic,
                IsCustom = false,
                IsSelected = false
            };
            skins["efe"] = efe;

            // Kai 皮肤
            SkinData kai = new SkinData
            {
                Name = "Kai",
                Author = "Mojang",
                Description = "默认 Kai 皮肤（细手臂）",
                SkinType = SkinType.Slim,
                IsCustom = false,
                IsSelected = false
            };
            skins["kai"] = kai;

            // Makena 皮肤
            SkinData makena = new SkinData
            {
                Name = "Makena",
                Author = "Mojang",
                Description = "默认 Makena 皮肤",
                SkinType = SkinType.Classic,
                IsCustom = false,
                IsSelected = false
            };
            skins["makena"] = makena;

            currentSkin = steve;
            currentSkinName = "steve";

            Console.WriteLine($"[SkinSystem] 已加载 {skins.Count} 个默认皮肤");
        }

        private void LoadCustomSkins()
        {
            if (!Directory.Exists(skinsDirectory)) return;

            string[] skinFiles = Directory.GetFiles(skinsDirectory, "*.png");
            foreach (string skinFile in skinFiles)
            {
                try
                {
                    string skinName = Path.GetFileNameWithoutExtension(skinFile);
                    if (!skins.ContainsKey(skinName))
                    {
                        SkinData skin = LoadSkinFromFile(skinFile);
                        if (skin != null)
                        {
                            skins[skinName] = skin;
                            OnSkinLoaded?.Invoke(skinName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SkinSystem] 加载皮肤失败: {skinFile} - {ex.Message}");
                }
            }
        }

        private SkinData LoadSkinFromFile(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // 检查皮肤尺寸
                // 标准皮肤：64x64 或 64x32
                // 这里简化处理，假设都是有效的皮肤文件

                return new SkinData
                {
                    Name = fileName,
                    Author = "自定义",
                    Description = $"自定义皮肤 ({fileInfo.Length / 1024} KB)",
                    FilePath = filePath,
                    SkinType = SkinType.Classic,
                    IsCustom = true,
                    IsSelected = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SkinSystem] 读取皮肤文件失败: {ex.Message}");
                return null;
            }
        }

        public bool SelectSkin(string skinName)
        {
            if (!skins.TryGetValue(skinName, out SkinData skin))
            {
                Console.WriteLine($"[SkinSystem] 未找到皮肤: {skinName}");
                return false;
            }

            // 取消之前选中的皮肤
            if (currentSkin != null)
            {
                currentSkin.IsSelected = false;
            }

            // 选中新皮肤
            skin.IsSelected = true;
            currentSkin = skin;
            currentSkinName = skinName;

            OnSkinChanged?.Invoke(skin);
            Console.WriteLine($"[SkinSystem] 已切换皮肤: {skin.Name}");
            return true;
        }

        public bool ImportSkin(string sourceFilePath, string skinName)
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine($"[SkinSystem] 源文件不存在: {sourceFilePath}");
                return false;
            }

            try
            {
                string destPath = Path.Combine(skinsDirectory, skinName + ".png");
                File.Copy(sourceFilePath, destPath, true);

                SkinData skin = LoadSkinFromFile(destPath);
                if (skin != null)
                {
                    skins[skinName] = skin;
                    OnSkinLoaded?.Invoke(skinName);
                    Console.WriteLine($"[SkinSystem] 已导入皮肤: {skinName}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SkinSystem] 导入皮肤失败: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSkin(string skinName)
        {
            if (!skins.TryGetValue(skinName, out SkinData skin))
            {
                return false;
            }

            if (!skin.IsCustom)
            {
                Console.WriteLine("[SkinSystem] 不能删除默认皮肤");
                return false;
            }

            try
            {
                if (!string.IsNullOrEmpty(skin.FilePath) && File.Exists(skin.FilePath))
                {
                    File.Delete(skin.FilePath);
                }

                skins.Remove(skinName);

                // 如果删除的是当前皮肤，切换到默认皮肤
                if (currentSkinName == skinName)
                {
                    SelectSkin("steve");
                }

                Console.WriteLine($"[SkinSystem] 已删除皮肤: {skinName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SkinSystem] 删除皮肤失败: {ex.Message}");
                return false;
            }
        }

        public SkinData GetSkin(string skinName)
        {
            if (skins.TryGetValue(skinName, out SkinData skin))
            {
                return skin;
            }
            return null;
        }

        public List<SkinData> GetAllSkins()
        {
            return new List<SkinData>(skins.Values);
        }

        public List<SkinData> GetCustomSkins()
        {
            List<SkinData> result = new List<SkinData>();
            foreach (SkinData skin in skins.Values)
            {
                if (skin.IsCustom)
                {
                    result.Add(skin);
                }
            }
            return result;
        }

        public List<SkinData> GetDefaultSkins()
        {
            List<SkinData> result = new List<SkinData>();
            foreach (SkinData skin in skins.Values)
            {
                if (!skin.IsCustom)
                {
                    result.Add(skin);
                }
            }
            return result;
        }

        public void SetSkinType(string skinName, SkinType type)
        {
            if (skins.TryGetValue(skinName, out SkinData skin))
            {
                skin.SkinType = type;
            }
        }

        public void RenderSkinPreview(UIManager uiManager, string skinName, int x, int y, int width, int height)
        {
            // 简化的皮肤预览渲染
            SkinData skin = GetSkin(skinName);
            if (skin == null) return;

            // 背景
            uiManager.DrawPanel(x, y, width, height,
                new Color4(0.2f, 0.2f, 0.25f, 1f));

            // 皮肤名称
            uiManager.DrawText(skin.Name, x + 5, y + 5, 12, Color4.White);

            // 皮肤类型
            string typeText = skin.SkinType == SkinType.Slim ? "细手臂" : "标准";
            uiManager.DrawText(typeText, x + 5, y + 20, 10,
                new Color4(0.7f, 0.7f, 0.7f, 1f));

            // 选中标记
            if (skin.IsSelected)
            {
                uiManager.DrawPanel(x, y + height - 20, width, 20,
                    new Color4(0.3f, 0.6f, 0.3f, 0.8f));
                int textWidth = uiManager.MeasureText("已选中", 10);
                uiManager.DrawText("已选中", x + (width - textWidth) / 2, y + height - 15, 10, Color4.White);
            }
        }

        public void RenderSkinSelector(UIManager uiManager, int x, int y, int width, int height)
        {
            int skinWidth = 80;
            int skinHeight = 100;
            int spacing = 10;
            int skinsPerRow = width / (skinWidth + spacing);

            List<SkinData> allSkins = GetAllSkins();

            for (int i = 0; i < allSkins.Count; i++)
            {
                int row = i / skinsPerRow;
                int col = i % skinsPerRow;
                int skinX = x + col * (skinWidth + spacing);
                int skinY = y + row * (skinHeight + spacing);

                if (skinY + skinHeight > y + height) break;

                RenderSkinPreview(uiManager, allSkins[i].Name, skinX, skinY, skinWidth, skinHeight);
            }
        }

        public string GetSkinsDirectory()
        {
            return skinsDirectory;
        }
    }

    public class SkinData
    {
        public string Name;
        public string Author;
        public string Description;
        public string FilePath;
        public SkinType SkinType;
        public bool IsCustom;
        public bool IsSelected;
        public DateTime LastModified;

        public override string ToString()
        {
            return Name;
        }
    }

    public enum SkinType
    {
        Classic, // 标准手臂（4像素宽）
        Slim     // 细手臂（3像素宽）
    }

    public class CapeSystem
    {
        private readonly Dictionary<string, CapeData> capes;
        private readonly string capesDirectory;
        private CapeData currentCape;
        private string currentCapeName;

        public int CapeCount => capes.Count;
        public CapeData CurrentCape => currentCape;

        public CapeSystem(string capesDirectory)
        {
            this.capesDirectory = capesDirectory;
            capes = new Dictionary<string, CapeData>();
        }

        public void Initialize()
        {
            Console.WriteLine("[CapeSystem] 披风系统初始化完成");

            if (!Directory.Exists(capesDirectory))
            {
                Directory.CreateDirectory(capesDirectory);
            }

            LoadDefaultCapes();
        }

        private void LoadDefaultCapes()
        {
            // 无披风
            capes["none"] = new CapeData
            {
                Name = "无披风",
                Description = "不显示披风",
                IsCustom = false,
                IsSelected = true
            };

            // Minecon 2011 披风
            capes["minecon2011"] = new CapeData
            {
                Name = "Minecon 2011",
                Description = "Minecon 2011 参会者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Minecon 2012 披风
            capes["minecon2012"] = new CapeData
            {
                Name = "Minecon 2012",
                Description = "Minecon 2012 参会者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Minecon 2013 披风
            capes["minecon2013"] = new CapeData
            {
                Name = "Minecon 2013",
                Description = "Minecon 2013 参会者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Minecon 2015 披风
            capes["minecon2015"] = new CapeData
            {
                Name = "Minecon 2015",
                Description = "Minecon 2015 参会者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Minecon 2016 披风
            capes["minecon2016"] = new CapeData
            {
                Name = "Minecon 2016",
                Description = "Minecon 2016 参会者披风",
                IsCustom = false,
                IsSelected = false
            };

            // 翻译者披风
            capes["translator"] = new CapeData
            {
                Name = "翻译者",
                Description = "Minecraft 翻译者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Mojang 披风
            capes["mojang"] = new CapeData
            {
                Name = "Mojang",
                Description = "Mojang 员工披风",
                IsCustom = false,
                IsSelected = false
            };

            //  Scrolls 披风
            capes["scrolls"] = new CapeData
            {
                Name = "Scrolls",
                Description = "Scrolls 测试者披风",
                IsCustom = false,
                IsSelected = false
            };

            // Cobalt 披风
            capes["cobalt"] = new CapeData
            {
                Name = "Cobalt",
                Description = "Cobalt 测试者披风",
                IsCustom = false,
                IsSelected = false
            };

            // 100万购买者披风
            capes["millionth"] = new CapeData
            {
                Name = "百万购买者",
                Description = "第100万位购买者披风",
                IsCustom = false,
                IsSelected = false
            };

            currentCape = capes["none"];
            currentCapeName = "none";

            Console.WriteLine($"[CapeSystem] 已加载 {capes.Count} 个披风");
        }

        public bool SelectCape(string capeName)
        {
            if (!capes.TryGetValue(capeName, out CapeData cape))
            {
                return false;
            }

            if (currentCape != null)
            {
                currentCape.IsSelected = false;
            }

            cape.IsSelected = true;
            currentCape = cape;
            currentCapeName = capeName;

            return true;
        }

        public CapeData GetCape(string capeName)
        {
            if (capes.TryGetValue(capeName, out CapeData cape))
            {
                return cape;
            }
            return null;
        }

        public List<CapeData> GetAllCapes()
        {
            return new List<CapeData>(capes.Values);
        }
    }

    public class CapeData
    {
        public string Name;
        public string Description;
        public string FilePath;
        public bool IsCustom;
        public bool IsSelected;

        public override string ToString()
        {
            return Name;
        }
    }
}
