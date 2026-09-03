using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Render
{
    public class SkyRenderer
    {
        private readonly ShaderManager shaderManager;
        private readonly Camera camera;
        private int skyVao;
        private int skyVbo;
        private int sunVao;
        private int sunVbo;
        private int moonVao;
        private int moonVbo;
        private int starVao;
        private int starVbo;
        private int starCount;

        // 天空颜色
        private Vector3 dayColor = new Vector3(0.4f, 0.6f, 0.9f);
        private Vector3 nightColor = new Vector3(0.02f, 0.02f, 0.08f);
        private Vector3 sunsetColor = new Vector3(0.9f, 0.5f, 0.3f);
        private Vector3 sunriseColor = new Vector3(0.9f, 0.6f, 0.4f);

        public SkyRenderer(ShaderManager shaderManager, Camera camera)
        {
            this.shaderManager = shaderManager;
            this.camera = camera;
        }

        public void Initialize()
        {
            CreateSkyDome();
            CreateSun();
            CreateMoon();
            CreateStars();
            Console.WriteLine("[SkyRenderer] 天空渲染器初始化完成");
        }

        private void CreateSkyDome()
        {
            // 简单的天空盒（大立方体）
            float size = 500f;
            float[] vertices = {
                -size, -size, -size,  size, -size, -size,  size,  size, -size, -size,  size, -size,
                -size, -size,  size,  size, -size,  size,  size,  size,  size, -size,  size,  size,
                -size,  size, -size,  size,  size, -size,  size,  size,  size, -size,  size,  size,
                -size, -size, -size,  size, -size, -size,  size, -size,  size, -size, -size,  size,
                 size, -size, -size,  size,  size, -size,  size,  size,  size,  size, -size,  size,
                -size, -size, -size, -size,  size, -size, -size,  size,  size, -size, -size,  size
            };

            skyVao = GL.GenVertexArray();
            skyVbo = GL.GenBuffer();

            GL.BindVertexArray(skyVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, skyVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void CreateSun()
        {
            // 太阳四边形
            float size = 30f;
            float[] vertices = {
                -size, -size, 0,
                 size, -size, 0,
                 size,  size, 0,
                -size,  size, 0
            };

            sunVao = GL.GenVertexArray();
            sunVbo = GL.GenBuffer();

            GL.BindVertexArray(sunVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, sunVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void CreateMoon()
        {
            float size = 20f;
            float[] vertices = {
                -size, -size, 0,
                 size, -size, 0,
                 size,  size, 0,
                -size,  size, 0
            };

            moonVao = GL.GenVertexArray();
            moonVbo = GL.GenBuffer();

            GL.BindVertexArray(moonVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, moonVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private void CreateStars()
        {
            Random random = new Random(42);
            starCount = 2000;
            float[] vertices = new float[starCount * 3];

            for (int i = 0; i < starCount; i++)
            {
                // 在天球上随机分布
                float theta = (float)(random.NextDouble() * Math.PI * 2);
                float phi = (float)(random.NextDouble() * Math.PI * 0.5); // 只在上半球
                float radius = 400f;

                vertices[i * 3] = (float)(radius * Math.Sin(phi) * Math.Cos(theta));
                vertices[i * 3 + 1] = (float)(radius * Math.Cos(phi));
                vertices[i * 3 + 2] = (float)(radius * Math.Sin(phi) * Math.Sin(theta));
            }

            starVao = GL.GenVertexArray();
            starVbo = GL.GenBuffer();

            GL.BindVertexArray(starVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, starVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        public void Render(float dayTime)
        {
            // dayTime: 0.0 - 1.0, 0.0 = 午夜, 0.25 = 日出, 0.5 = 正午, 0.75 = 日落

            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            RenderSkyDome(dayTime);
            RenderStars(dayTime);
            RenderSun(dayTime);
            RenderMoon(dayTime);

            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
        }

        private void RenderSkyDome(float dayTime)
        {
            Shader skyShader = shaderManager.GetShader("sky");
            if (skyShader == null) return;

            skyShader.Use();

            // 计算天空颜色
            Vector3 skyColor = GetSkyColor(dayTime);

            Matrix4 view = camera.GetViewMatrix();
            view.ClearTranslation(); // 天空不随相机移动
            Matrix4 projection = camera.GetProjectionMatrix();

            skyShader.SetMatrix4("view", view);
            skyShader.SetMatrix4("projection", projection);
            skyShader.SetVector3("skyColor", skyColor);
            skyShader.SetFloat("dayTime", dayTime);

            GL.BindVertexArray(skyVao);
            GL.DrawArrays(PrimitiveType.Quads, 0, 24);
            GL.BindVertexArray(0);

            skyShader.Unbind();
        }

        private void RenderSun(float dayTime)
        {
            Shader sunShader = shaderManager.GetShader("sun");
            if (sunShader == null) return;

            // 太阳位置
            float angle = dayTime * (float)Math.PI * 2 - (float)Math.PI / 2;
            Vector3 sunPos = new Vector3(
                (float)Math.Cos(angle) * 300,
                (float)Math.Sin(angle) * 300,
                0
            );

            if (sunPos.Y < -50) return; // 太阳在地平线下

            sunShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            view.ClearTranslation();
            Matrix4 projection = camera.GetProjectionMatrix();
            Matrix4 model = Matrix4.CreateTranslation(sunPos);

            // 让太阳始终面向相机
            model *= Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(camera.Rotation));

            sunShader.SetMatrix4("view", view);
            sunShader.SetMatrix4("projection", projection);
            sunShader.SetMatrix4("model", model);
            sunShader.SetVector4("color", new Vector4(1.0f, 0.9f, 0.6f, 1.0f));

            GL.BindVertexArray(sunVao);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);

            sunShader.Unbind();
        }

        private void RenderMoon(float dayTime)
        {
            Shader moonShader = shaderManager.GetShader("moon");
            if (moonShader == null) return;

            // 月亮位置（与太阳相反）
            float angle = dayTime * (float)Math.PI * 2 + (float)Math.PI / 2;
            Vector3 moonPos = new Vector3(
                (float)Math.Cos(angle) * 280,
                (float)Math.Sin(angle) * 280,
                0
            );

            if (moonPos.Y < -50) return;

            moonShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            view.ClearTranslation();
            Matrix4 projection = camera.GetProjectionMatrix();
            Matrix4 model = Matrix4.CreateTranslation(moonPos);
            model *= Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(camera.Rotation));

            moonShader.SetMatrix4("view", view);
            moonShader.SetMatrix4("projection", projection);
            moonShader.SetMatrix4("model", model);
            moonShader.SetVector4("color", new Vector4(0.9f, 0.9f, 1.0f, 1.0f));

            GL.BindVertexArray(moonVao);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);

            moonShader.Unbind();
        }

        private void RenderStars(float dayTime)
        {
            Shader starShader = shaderManager.GetShader("star");
            if (starShader == null) return;

            // 只在夜晚显示星星
            float starAlpha = GetStarAlpha(dayTime);
            if (starAlpha <= 0.01f) return;

            starShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            view.ClearTranslation();
            Matrix4 projection = camera.GetProjectionMatrix();

            starShader.SetMatrix4("view", view);
            starShader.SetMatrix4("projection", projection);
            starShader.SetFloat("alpha", starAlpha);

            GL.PointSize(2.0f);
            GL.BindVertexArray(starVao);
            GL.DrawArrays(PrimitiveType.Points, 0, starCount);
            GL.BindVertexArray(0);

            starShader.Unbind();
        }

        public Vector3 GetSkyColor(float dayTime)
        {
            // 日出
            if (dayTime > 0.2f && dayTime < 0.3f)
            {
                float t = (dayTime - 0.2f) / 0.1f;
                return Vector3.Lerp(nightColor, sunriseColor, t);
            }
            // 上午
            if (dayTime >= 0.3f && dayTime < 0.45f)
            {
                float t = (dayTime - 0.3f) / 0.15f;
                return Vector3.Lerp(sunriseColor, dayColor, t);
            }
            // 白天
            if (dayTime >= 0.45f && dayTime < 0.55f)
            {
                return dayColor;
            }
            // 下午
            if (dayTime >= 0.55f && dayTime < 0.7f)
            {
                float t = (dayTime - 0.55f) / 0.15f;
                return Vector3.Lerp(dayColor, sunsetColor, t);
            }
            // 日落
            if (dayTime >= 0.7f && dayTime < 0.8f)
            {
                float t = (dayTime - 0.7f) / 0.1f;
                return Vector3.Lerp(sunsetColor, nightColor, t);
            }
            // 夜晚
            return nightColor;
        }

        public float GetStarAlpha(float dayTime)
        {
            if (dayTime < 0.2f || dayTime > 0.8f)
            {
                return 1.0f;
            }
            if (dayTime >= 0.2f && dayTime < 0.3f)
            {
                return 1.0f - (dayTime - 0.2f) / 0.1f;
            }
            if (dayTime >= 0.7f && dayTime < 0.8f)
            {
                return (dayTime - 0.7f) / 0.1f;
            }
            return 0f;
        }

        public float GetLightIntensity(float dayTime)
        {
            Vector3 skyColor = GetSkyColor(dayTime);
            return (skyColor.X + skyColor.Y + skyColor.Z) / 3.0f;
        }

        public void Dispose()
        {
            if (skyVao != 0)
            {
                GL.DeleteVertexArray(skyVao);
                GL.DeleteBuffer(skyVbo);
                GL.DeleteVertexArray(sunVao);
                GL.DeleteBuffer(sunVbo);
                GL.DeleteVertexArray(moonVao);
                GL.DeleteBuffer(moonVbo);
                GL.DeleteVertexArray(starVao);
                GL.DeleteBuffer(starVbo);
            }
            Console.WriteLine("[SkyRenderer] 天空渲染器已释放");
        }
    }
}
