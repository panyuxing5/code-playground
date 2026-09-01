using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class TexturePackManager
    {
        private readonly List<TexturePack> packs;
        private readonly string packsDirectory;
        private TexturePack currentPack;
        private string currentPackName;

        // 事件
        public event Action<TexturePack> OnPackChanged;
        public event Action<string> OnPackLoaded;

        public int PackCount => packs.Count;
        public TexturePack CurrentPack => currentPack;
        public string CurrentPackName => currentPackName;

        public TexturePackManager(string packsDirectory)
        {
            this.packsDirectory = packsDirectory;
            packs = new List<TexturePack>();
        }

        public void Initialize()
        {
            Console.WriteLine("[TexturePackManager] 材质包管理器初始化完成");
            Console.WriteLine($"[TexturePackManager] 材质包目录: {packsDirectory}");

            if (!Directory.Exists(packsDirectory))
            {
                Directory.CreateDirectory(packsDirectory);
                Console.WriteLine("[TexturePackManager] 已创建材质包目录");
            }

            LoadDefaultPack();
            LoadCustomPacks();
        }

        private void LoadDefaultPack()
        {
            TexturePack defaultPack = new TexturePack
            {
                Name = "默认材质包",
                Description = "VoxelCraft 默认材质包",
                Version = "1.0.0",
                Author = "VoxelCraft Team",
                Directory = "default",
                IsEnabled = true,
                IsDefault = true,
                Priority = 0,
                TextureResolution = 16
            };

            packs.Add(defaultPack);
            currentPack = defaultPack;
            currentPackName = "default";

            Console.WriteLine("[TexturePackManager] 已加载默认材质包");
        }

        private void LoadCustomPacks()
        {
            if (!Directory.Exists(packsDirectory)) return;

            string[] packDirs = Directory.GetDirectories(packsDirectory);
            foreach (string packDir in packDirs)
            {
                try
                {
                    string packName = Path.GetFileName(packDir);
                    if (!packs.Exists(p => p.Name == packName))
                    {
                        TexturePack pack = LoadPackFromDirectory(packDir);
                        if (pack != null)
                        {
                            packs.Add(pack);
                            OnPackLoaded?.Invoke(packName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TexturePackManager] 加载材质包失败: {packDir} - {ex.Message}");
                }
            }

            // 加载zip格式的材质包
            string[] zipFiles = Directory.GetFiles(packsDirectory, "*.zip");
            foreach (string zipFile in zipFiles)
            {
                try
                {
                    string packName = Path.GetFileNameWithoutExtension(zipFile);
                    if (!packs.Exists(p => p.Name == packName))
                    {
                        TexturePack pack = LoadPackFromZip(zipFile);
                        if (pack != null)
                        {
                            packs.Add(pack);
                            OnPackLoaded?.Invoke(packName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TexturePackManager] 加载材质包失败: {zipFile} - {ex.Message}");
                }
            }
        }

        private TexturePack LoadPackFromDirectory(string directory)
        {
            try
            {
                string metaPath = Path.Combine(directory, "pack.mcmeta");
                string packName = Path.GetFileName(directory);

                TexturePack pack = new TexturePack
                {
                    Name = packName,
                    Description = "自定义材质包",
                    Version = "1.0.0",
                    Author = "自定义",
                    Directory = directory,
                    IsEnabled = false,
                    IsDefault = false,
                    Priority = packs.Count,
                    TextureResolution = 16
                };

                // 读取pack.mcmeta
                if (File.Exists(metaPath))
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

                // 检测材质分辨率
                string texturesPath = Path.Combine(directory, "assets", "textures");
                if (Directory.Exists(texturesPath))
                {
                    string[] blockTextures = Directory.GetFiles(texturesPath, "*.png", SearchOption.AllDirectories);
                    if (blockTextures.Length > 0)
                    {
                        // 简化：假设都是相同分辨率
                        pack.TextureResolution = 16;
                    }
                }

                return pack;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TexturePackManager] 读取材质包目录失败: {ex.Message}");
                return null;
            }
        }

        private TexturePack LoadPackFromZip(string zipPath)
        {
            try
            {
                string packName = Path.GetFileNameWithoutExtension(zipPath);

                TexturePack pack = new TexturePack
                {
                    Name = packName,
                    Description = "自定义材质包 (ZIP)",
                    Version = "1.0.0",
                    Author = "自定义",
                    Directory = zipPath,
                    IsEnabled = false,
                    IsDefault = false,
                    Priority = packs.Count,
                    TextureResolution = 16,
                    IsZip = true
                };

                return pack;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TexturePackManager] 读取ZIP材质包失败: {ex.Message}");
                return null;
            }
        }

        public bool SelectPack(string packName)
        {
            TexturePack pack = packs.Find(p => p.Name == packName);
            if (pack == null)
            {
                Console.WriteLine($"[TexturePackManager] 未找到材质包: {packName}");
                return false;
            }

            if (currentPack != null)
            {
                currentPack.IsEnabled = false;
            }

            pack.IsEnabled = true;
            currentPack = pack;
            currentPackName = packName;

            OnPackChanged?.Invoke(pack);
            Console.WriteLine($"[TexturePackManager] 已切换材质包: {pack.Name}");
            return true;
        }

        public bool ImportPack(string sourcePath, string packName)
        {
            if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
            {
                Console.WriteLine($"[TexturePackManager] 源文件不存在: {sourcePath}");
                return false;
            }

            try
            {
                string destPath = Path.Combine(packsDirectory, packName);

                if (Directory.Exists(sourcePath))
                {
                    // 复制目录
                    CopyDirectory(sourcePath, destPath);
                }
                else if (File.Exists(sourcePath))
                {
                    // 复制文件
                    string ext = Path.GetExtension(sourcePath);
                    File.Copy(sourcePath, destPath + ext, true);
                }

                // 重新加载
                packs.Clear();
                LoadDefaultPack();
                LoadCustomPacks();

                Console.WriteLine($"[TexturePackManager] 已导入材质包: {packName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TexturePackManager] 导入材质包失败: {ex.Message}");
                return false;
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                string destSubDir = Path.Combine(destDir, dirName);
                CopyDirectory(dir, destSubDir);
            }
        }

        public bool DeletePack(string packName)
        {
            TexturePack pack = packs.Find(p => p.Name == packName);
            if (pack == null)
            {
                return false;
            }

            if (pack.IsDefault)
            {
                Console.WriteLine("[TexturePackManager] 不能删除默认材质包");
                return false;
            }

            try
            {
                if (pack.IsZip)
                {
                    if (File.Exists(pack.Directory))
                    {
                        File.Delete(pack.Directory);
                    }
                }
                else
                {
                    if (Directory.Exists(pack.Directory))
                    {
                        Directory.Delete(pack.Directory, true);
                    }
                }

                packs.Remove(pack);

                if (currentPackName == packName)
                {
                    SelectPack("默认材质包");
                }

                Console.WriteLine($"[TexturePackManager] 已删除材质包: {packName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TexturePackManager] 删除材质包失败: {ex.Message}");
                return false;
            }
        }

        public TexturePack GetPack(string packName)
        {
            return packs.Find(p => p.Name == packName);
        }

        public List<TexturePack> GetAllPacks()
        {
            return new List<TexturePack>(packs);
        }

        public List<TexturePack> GetEnabledPacks()
        {
            return packs.FindAll(p => p.IsEnabled);
        }

        public void SetPackPriority(string packName, int priority)
        {
            TexturePack pack = packs.Find(p => p.Name == packName);
            if (pack != null)
            {
                pack.Priority = priority;
                packs.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }

        public void MovePackUp(string packName)
        {
            int index = packs.FindIndex(p => p.Name == packName);
            if (index > 1) // 0是默认包
            {
                TexturePack temp = packs[index];
                packs[index] = packs[index - 1];
                packs[index - 1] = temp;

                for (int i = 0; i < packs.Count; i++)
                {
                    packs[i].Priority = i;
                }
            }
        }

        public void MovePackDown(string packName)
        {
            int index = packs.FindIndex(p => p.Name == packName);
            if (index >= 0 && index < packs.Count - 1)
            {
                TexturePack temp = packs[index];
                packs[index] = packs[index + 1];
                packs[index + 1] = temp;

                for (int i = 0; i < packs.Count; i++)
                {
                    packs[i].Priority = i;
                }
            }
        }

        public void ReloadPacks()
        {
            Console.WriteLine("[TexturePackManager] 重新加载材质包...");

            packs.Clear();
            LoadDefaultPack();
            LoadCustomPacks();

            Console.WriteLine("[TexturePackManager] 材质包重新加载完成");
        }

        public string GetTexturePath(string textureKey)
        {
            if (currentPack == null || currentPack.IsDefault)
            {
                return null;
            }

            // 从当前材质包查找纹理
            string texturePath = Path.Combine(currentPack.Directory, "assets", "textures", textureKey + ".png");
            if (File.Exists(texturePath))
            {
                return texturePath;
            }

            return null;
        }

        public string GetPacksDirectory()
        {
            return packsDirectory;
        }

        public void RenderPackSelector(UIManager uiManager, int x, int y, int width, int height)
        {
            int packWidth = 150;
            int packHeight = 120;
            int spacing = 10;
            int packsPerRow = width / (packWidth + spacing);

            for (int i = 0; i < packs.Count; i++)
            {
                int row = i / packsPerRow;
                int col = i % packsPerRow;
                int packX = x + col * (packWidth + spacing);
                int packY = y + row * (packHeight + spacing);

                if (packY + packHeight > y + height) break;

                RenderPackPreview(uiManager, packs[i], packX, packY, packWidth, packHeight);
            }
        }

        private void RenderPackPreview(UIManager uiManager, TexturePack pack, int x, int y, int width, int height)
        {
            // 背景
            uiManager.DrawPanel(x, y, width, height,
                pack.IsEnabled ? new Color4(0.3f, 0.3f, 0.4f, 0.9f) : new Color4(0.2f, 0.2f, 0.25f, 0.9f));

            // 材质包名称
            uiManager.DrawText(pack.Name, x + 5, y + 5, 12, Color4.White);

            // 描述
            uiManager.DrawText(pack.Description, x + 5, y + 25, 10,
                new Color4(0.7f, 0.7f, 0.7f, 1f));

            // 分辨率
            uiManager.DrawText($"{pack.TextureResolution}x", x + 5, y + 45, 10,
                new Color4(0.6f, 0.6f, 0.6f, 1f));

            // 作者
            uiManager.DrawText(pack.Author, x + 5, y + 60, 10,
                new Color4(0.6f, 0.6f, 0.6f, 1f));

            // 版本
            uiManager.DrawText($"v{pack.Version}", x + 5, y + 75, 10,
                new Color4(0.5f, 0.5f, 0.5f, 1f));

            // 选中标记
            if (pack.IsEnabled)
            {
                uiManager.DrawPanel(x, y + height - 20, width, 20,
                    new Color4(0.3f, 0.6f, 0.3f, 0.8f));
                int textWidth = uiManager.MeasureText("已启用", 10);
                uiManager.DrawText("已启用", x + (width - textWidth) / 2, y + height - 15, 10, Color4.White);
            }
        }
    }

    public class TexturePack
    {
        public string Name;
        public string Description;
        public string Version;
        public string Author;
        public string Directory;
        public bool IsEnabled;
        public bool IsDefault;
        public int Priority;
        public int TextureResolution;
        public int PackFormat;
        public bool IsZip;

        public override string ToString()
        {
            return Name;
        }
    }

    public class ShaderManager
    {
        private readonly Dictionary<string, ShaderProgram> shaders;
        private readonly string shadersDirectory;
        private ShaderProgram currentShader;

        public int ShaderCount => shaders.Count;
        public ShaderProgram CurrentShader => currentShader;

        public ShaderManager(string shadersDirectory)
        {
            this.shadersDirectory = shadersDirectory;
            shaders = new Dictionary<string, ShaderProgram>();
        }

        public void Initialize()
        {
            Console.WriteLine("[ShaderManager] 着色器管理器初始化完成");
            LoadDefaultShaders();
        }

        private void LoadDefaultShaders()
        {
            // 方块着色器
            shaders["block"] = new ShaderProgram
            {
                Name = "block",
                VertexShader = DefaultBlockVertexShader,
                FragmentShader = DefaultBlockFragmentShader,
                IsEnabled = true
            };

            // 实体着色器
            shaders["entity"] = new ShaderProgram
            {
                Name = "entity",
                VertexShader = DefaultEntityVertexShader,
                FragmentShader = DefaultEntityFragmentShader,
                IsEnabled = true
            };

            // 粒子着色器
            shaders["particle"] = new ShaderProgram
            {
                Name = "particle",
                VertexShader = DefaultParticleVertexShader,
                FragmentShader = DefaultParticleFragmentShader,
                IsEnabled = true
            };

            // 天空着色器
            shaders["sky"] = new ShaderProgram
            {
                Name = "sky",
                VertexShader = DefaultSkyVertexShader,
                FragmentShader = DefaultSkyFragmentShader,
                IsEnabled = true
            };

            // UI着色器
            shaders["ui"] = new ShaderProgram
            {
                Name = "ui",
                VertexShader = DefaultUIVertexShader,
                FragmentShader = DefaultUIFragmentShader,
                IsEnabled = true
            };

            // 水着色器
            shaders["water"] = new ShaderProgram
            {
                Name = "water",
                VertexShader = DefaultWaterVertexShader,
                FragmentShader = DefaultWaterFragmentShader,
                IsEnabled = true
            };

            // 光照着色器
            shaders["lighting"] = new ShaderProgram
            {
                Name = "lighting",
                VertexShader = DefaultLightingVertexShader,
                FragmentShader = DefaultLightingFragmentShader,
                IsEnabled = true
            };

            // 阴影着色器
            shaders["shadow"] = new ShaderProgram
            {
                Name = "shadow",
                VertexShader = DefaultShadowVertexShader,
                FragmentShader = DefaultShadowFragmentShader,
                IsEnabled = true
            };

            // 后期处理着色器
            shaders["postprocess"] = new ShaderProgram
            {
                Name = "postprocess",
                VertexShader = DefaultPostProcessVertexShader,
                FragmentShader = DefaultPostProcessFragmentShader,
                IsEnabled = true
            };

            currentShader = shaders["block"];

            Console.WriteLine($"[ShaderManager] 已加载 {shaders.Count} 个着色器程序");
        }

        public bool UseShader(string shaderName)
        {
            if (shaders.TryGetValue(shaderName, out ShaderProgram shader))
            {
                currentShader = shader;
                return true;
            }
            return false;
        }

        public ShaderProgram GetShader(string shaderName)
        {
            if (shaders.TryGetValue(shaderName, out ShaderProgram shader))
            {
                return shader;
            }
            return null;
        }

        public List<string> GetAllShaderNames()
        {
            return new List<string>(shaders.Keys);
        }

        public void ReloadShaders()
        {
            Console.WriteLine("[ShaderManager] 重新加载着色器...");
            shaders.Clear();
            LoadDefaultShaders();
            Console.WriteLine("[ShaderManager] 着色器重新加载完成");
        }

        // 默认着色器代码（简化版）
        private const string DefaultBlockVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 2) in vec3 aNormal;
            layout(location = 3) in vec3 aColor;

            uniform mat4 uProjection;
            uniform mat4 uView;
            uniform mat4 uModel;

            out vec2 vTexCoord;
            out vec3 vNormal;
            out vec3 vColor;
            out vec3 vWorldPos;

            void main() {
                vec4 worldPos = uModel * vec4(aPosition, 1.0);
                vWorldPos = worldPos.xyz;
                vTexCoord = aTexCoord;
                vNormal = mat3(transpose(inverse(uModel))) * aNormal;
                vColor = aColor;
                gl_Position = uProjection * uView * worldPos;
            }
        ";

        private const string DefaultBlockFragmentShader = @"
            #version 330 core
            in vec2 vTexCoord;
            in vec3 vNormal;
            in vec3 vColor;
            in vec3 vWorldPos;

            uniform sampler2D uTexture;
            uniform vec3 uLightDir;
            uniform vec3 uLightColor;
            uniform vec3 uAmbientColor;
            uniform float uAmbientStrength;

            out vec4 FragColor;

            void main() {
                vec4 texColor = texture(uTexture, vTexCoord);
                vec3 normal = normalize(vNormal);
                vec3 lightDir = normalize(uLightDir);

                float diff = max(dot(normal, lightDir), 0.0);
                vec3 diffuse = diff * uLightColor;
                vec3 ambient = uAmbientStrength * uAmbientColor;

                vec3 result = (ambient + diffuse) * texColor.rgb * vColor;
                FragColor = vec4(result, texColor.a);
            }
        ";

        private const string DefaultEntityVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 2) in vec3 aNormal;

            uniform mat4 uProjection;
            uniform mat4 uView;
            uniform mat4 uModel;

            out vec2 vTexCoord;
            out vec3 vNormal;

            void main() {
                vTexCoord = aTexCoord;
                vNormal = mat3(transpose(inverse(uModel))) * aNormal;
                gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
            }
        ";

        private const string DefaultEntityFragmentShader = @"
            #version 330 core
            in vec2 vTexCoord;
            in vec3 vNormal;

            uniform sampler2D uTexture;
            uniform vec3 uLightDir;

            out vec4 FragColor;

            void main() {
                vec4 texColor = texture(uTexture, vTexCoord);
                vec3 normal = normalize(vNormal);
                vec3 lightDir = normalize(uLightDir);
                float diff = max(dot(normal, lightDir), 0.0);
                vec3 result = (0.3 + 0.7 * diff) * texColor.rgb;
                FragColor = vec4(result, texColor.a);
            }
        ";

        private const string DefaultParticleVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec4 aColor;
            layout(location = 2) in float aSize;

            uniform mat4 uProjection;
            uniform mat4 uView;

            out vec4 vColor;

            void main() {
                vColor = aColor;
                gl_PointSize = aSize;
                gl_Position = uProjection * uView * vec4(aPosition, 1.0);
            }
        ";

        private const string DefaultParticleFragmentShader = @"
            #version 330 core
            in vec4 vColor;
            out vec4 FragColor;

            void main() {
                vec2 coord = gl_PointCoord - vec2(0.5);
                if (length(coord) > 0.5) discard;
                FragColor = vColor;
            }
        ";

        private const string DefaultSkyVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 uProjection;
            uniform mat4 uView;
            out vec3 vPosition;

            void main() {
                vPosition = aPosition;
                mat4 viewNoTranslation = mat4(mat3(uView));
                gl_Position = uProjection * viewNoTranslation * vec4(aPosition, 1.0);
            }
        ";

        private const string DefaultSkyFragmentShader = @"
            #version 330 core
            in vec3 vPosition;
            uniform vec3 uSkyColor;
            uniform vec3 uHorizonColor;
            uniform float uTime;
            out vec4 FragColor;

            void main() {
                float y = normalize(vPosition).y;
                float t = max(y, 0.0);
                vec3 color = mix(uHorizonColor, uSkyColor, t);
                FragColor = vec4(color, 1.0);
            }
        ";

        private const string DefaultUIVertexShader = @"
            #version 330 core
            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 2) in vec4 aColor;

            uniform vec2 uScreenSize;

            out vec2 vTexCoord;
            out vec4 vColor;

            void main() {
                vec2 pos = aPosition / uScreenSize * 2.0 - 1.0;
                pos.y = -pos.y;
                vTexCoord = aTexCoord;
                vColor = aColor;
                gl_Position = vec4(pos, 0.0, 1.0);
            }
        ";

        private const string DefaultUIFragmentShader = @"
            #version 330 core
            in vec2 vTexCoord;
            in vec4 vColor;
            uniform sampler2D uTexture;
            uniform bool uUseTexture;
            out vec4 FragColor;

            void main() {
                if (uUseTexture) {
                    FragColor = texture(uTexture, vTexCoord) * vColor;
                } else {
                    FragColor = vColor;
                }
            }
        ";

        private const string DefaultWaterVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            uniform mat4 uProjection;
            uniform mat4 uView;
            uniform mat4 uModel;
            uniform float uTime;
            out vec2 vTexCoord;
            out vec3 vWorldPos;

            void main() {
                vec3 pos = aPosition;
                pos.y += sin(pos.x * 0.5 + uTime) * 0.05;
                pos.y += cos(pos.z * 0.5 + uTime) * 0.05;
                vTexCoord = aTexCoord;
                vec4 worldPos = uModel * vec4(pos, 1.0);
                vWorldPos = worldPos.xyz;
                gl_Position = uProjection * uView * worldPos;
            }
        ";

        private const string DefaultWaterFragmentShader = @"
            #version 330 core
            in vec2 vTexCoord;
            in vec3 vWorldPos;
            uniform sampler2D uTexture;
            uniform float uTime;
            out vec4 FragColor;

            void main() {
                vec2 uv = vTexCoord;
                uv.x += sin(vWorldPos.x * 0.1 + uTime) * 0.02;
                uv.y += cos(vWorldPos.z * 0.1 + uTime) * 0.02;
                vec4 color = texture(uTexture, uv);
                color.a = 0.6;
                FragColor = color;
            }
        ";

        private const string DefaultLightingVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            layout(location = 1) in vec3 aNormal;
            uniform mat4 uProjection;
            uniform mat4 uView;
            uniform mat4 uModel;
            out vec3 vNormal;
            out vec3 vWorldPos;

            void main() {
                vec4 worldPos = uModel * vec4(aPosition, 1.0);
                vWorldPos = worldPos.xyz;
                vNormal = mat3(transpose(inverse(uModel))) * aNormal;
                gl_Position = uProjection * uView * worldPos;
            }
        ";

        private const string DefaultLightingFragmentShader = @"
            #version 330 core
            in vec3 vNormal;
            in vec3 vWorldPos;
            uniform vec3 uLightPos;
            uniform vec3 uLightColor;
            uniform vec3 uViewPos;
            uniform float uShininess;
            out vec4 FragColor;

            void main() {
                vec3 normal = normalize(vNormal);
                vec3 lightDir = normalize(uLightPos - vWorldPos);
                vec3 viewDir = normalize(uViewPos - vWorldPos);
                vec3 reflectDir = reflect(-lightDir, normal);

                float diff = max(dot(normal, lightDir), 0.0);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), uShininess);

                vec3 ambient = 0.1 * uLightColor;
                vec3 diffuse = diff * uLightColor;
                vec3 specular = spec * uLightColor;

                FragColor = vec4(ambient + diffuse + specular, 1.0);
            }
        ";

        private const string DefaultShadowVertexShader = @"
            #version 330 core
            layout(location = 0) in vec3 aPosition;
            uniform mat4 uLightSpace;
            uniform mat4 uModel;

            void main() {
                gl_Position = uLightSpace * uModel * vec4(aPosition, 1.0);
            }
        ";

        private const string DefaultShadowFragmentShader = @"
            #version 330 core
            void main() {
                gl_FragDepth = gl_FragCoord.z;
            }
        ";

        private const string DefaultPostProcessVertexShader = @"
            #version 330 core
            layout(location = 0) in vec2 aPosition;
            layout(location = 1) in vec2 aTexCoord;
            out vec2 vTexCoord;

            void main() {
                vTexCoord = aTexCoord;
                gl_Position = vec4(aPosition, 0.0, 1.0);
            }
        ";

        private const string DefaultPostProcessFragmentShader = @"
            #version 330 core
            in vec2 vTexCoord;
            uniform sampler2D uScene;
            uniform float uExposure;
            uniform float uGamma;
            out vec4 FragColor;

            void main() {
                vec3 color = texture(uScene, vTexCoord).rgb;
                color = vec3(1.0) - exp(-color * uExposure);
                color = pow(color, vec3(1.0 / uGamma));
                FragColor = vec4(color, 1.0);
            }
        ";
    }

    public class ShaderProgram
    {
        public string Name;
        public string VertexShader;
        public string FragmentShader;
        public string GeometryShader;
        public bool IsEnabled;
        public int ProgramId;

        public override string ToString()
        {
            return Name;
        }
    }
}
