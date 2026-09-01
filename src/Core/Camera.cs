using System;
using OpenTK.Mathematics;

namespace VoxelCraft.Core
{
    public class Camera
    {
        // ========================================
        // 位置和朝向
        // ========================================
        public Vector3 Position { get; set; } = Vector3.Zero;
        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; } = 0f;
        public float Roll { get; set; } = 0f;

        // ========================================
        // 相机向量
        // ========================================
        public Vector3 Forward { get; private set; } = -Vector3.UnitZ;
        public Vector3 Right { get; private set; } = Vector3.UnitX;
        public Vector3 Up { get; private set; } = Vector3.UnitY;

        // ========================================
        // 投影参数
        // ========================================
        public float FOV { get; private set; } = 70f;
        public float AspectRatio { get; private set; } = 16f / 9f;
        public float NearPlane { get; private set; } = 0.1f;
        public float FarPlane { get; private set; } = 1000f;

        // ========================================
        // 矩阵
        // ========================================
        public Matrix4 ViewMatrix { get; private set; }
        public Matrix4 ProjectionMatrix { get; private set; }
        public Matrix4 ViewProjectionMatrix { get; private set; }
        public Matrix4 InverseViewMatrix { get; private set; }
        public Matrix4 InverseProjectionMatrix { get; private set; }

        // ========================================
        // 视锥体
        // ========================================
        private Plane[] frustumPlanes = new Plane[6];

        // ========================================
        // 相机抖动
        // ========================================
        public bool UseBobEffect { get; set; } = true;
        public float BobAmount { get; set; } = 0.05f;
        public float BobSpeed { get; set; } = 10f;
        private float bobTimer = 0f;
        private float bobOffset = 0f;

        // ========================================
        // 相机平滑
        // ========================================
        public bool UseSmoothCamera { get; set; } = false;
        public float SmoothSpeed { get; set; } = 5f;
        private Vector3 smoothPosition;
        private float smoothYaw;
        private float smoothPitch;

        // ========================================
        // 正交投影（UI用）
        // ========================================
        public Matrix4 OrthographicMatrix { get; private set; }
        public int ScreenWidth { get; private set; } = 1280;
        public int ScreenHeight { get; private set; } = 720;

        // ========================================
        // 构造函数
        // ========================================
        public Camera()
        {
            UpdateCameraVectors();
            UpdateViewMatrix();
            UpdateProjectionMatrix(FOV, AspectRatio, NearPlane, FarPlane);
        }

        public Camera(Vector3 position, float yaw, float pitch)
        {
            Position = position;
            Yaw = yaw;
            Pitch = pitch;
            smoothPosition = position;
            smoothYaw = yaw;
            smoothPitch = pitch;
            UpdateCameraVectors();
            UpdateViewMatrix();
            UpdateProjectionMatrix(FOV, AspectRatio, NearPlane, FarPlane);
        }

        // ========================================
        // 更新相机向量
        // ========================================
        public void UpdateCameraVectors()
        {
            // 计算前向向量
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            Forward = new Vector3(
                (float)(Math.Cos(pitchRad) * Math.Cos(yawRad)),
                (float)Math.Sin(pitchRad),
                (float)(Math.Cos(pitchRad) * Math.Sin(yawRad))
            ).Normalized();

            // 计算右向量
            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));

            // 计算上向量
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        }

        // ========================================
        // 更新视图矩阵
        // ========================================
        public void UpdateViewMatrix()
        {
            Vector3 cameraPos = Position;

            // 应用相机抖动
            if (UseBobEffect && bobOffset != 0f)
            {
                cameraPos.Y += bobOffset;
            }

            // 应用平滑相机
            if (UseSmoothCamera)
            {
                cameraPos = smoothPosition;
            }

            ViewMatrix = Matrix4.LookAt(cameraPos, cameraPos + Forward, Up);
            InverseViewMatrix = Matrix4.Invert(ViewMatrix);
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
            UpdateFrustumPlanes();
        }

        // ========================================
        // 更新投影矩阵
        // ========================================
        public void UpdateProjectionMatrix(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            FOV = fov;
            AspectRatio = aspectRatio;
            NearPlane = nearPlane;
            FarPlane = farPlane;

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fov),
                aspectRatio,
                nearPlane,
                farPlane
            );

            InverseProjectionMatrix = Matrix4.Invert(ProjectionMatrix);
            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
            UpdateFrustumPlanes();
        }

        // ========================================
        // 更新正交投影矩阵
        // ========================================
        public void UpdateOrthographicMatrix(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            OrthographicMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
        }

        // ========================================
        // 更新视锥体平面
        // ========================================
        private void UpdateFrustumPlanes()
        {
            Matrix4 m = ViewProjectionMatrix;

            // 左平面
            frustumPlanes[0] = new Plane(
                m.M14 + m.M11,
                m.M24 + m.M21,
                m.M34 + m.M31,
                m.M44 + m.M41
            );

            // 右平面
            frustumPlanes[1] = new Plane(
                m.M14 - m.M11,
                m.M24 - m.M21,
                m.M34 - m.M31,
                m.M44 - m.M41
            );

            // 底平面
            frustumPlanes[2] = new Plane(
                m.M14 + m.M12,
                m.M24 + m.M22,
                m.M34 + m.M32,
                m.M44 + m.M42
            );

            // 顶平面
            frustumPlanes[3] = new Plane(
                m.M14 - m.M12,
                m.M24 - m.M22,
                m.M34 - m.M32,
                m.M44 - m.M42
            );

            // 近平面
            frustumPlanes[4] = new Plane(
                m.M14 + m.M13,
                m.M24 + m.M23,
                m.M34 + m.M33,
                m.M44 + m.M43
            );

            // 远平面
            frustumPlanes[5] = new Plane(
                m.M14 - m.M13,
                m.M24 - m.M23,
                m.M34 - m.M33,
                m.M44 - m.M43
            );

            // 归一化所有平面
            for (int i = 0; i < 6; i++)
            {
                frustumPlanes[i].Normalize();
            }
        }

        // ========================================
        // 视锥体剔除
        // ========================================
        public bool IsPointInFrustum(Vector3 point)
        {
            for (int i = 0; i < 6; i++)
            {
                if (frustumPlanes[i].GetDistanceToPoint(point) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsBoxInFrustum(Vector3 min, Vector3 max)
        {
            for (int i = 0; i < 6; i++)
            {
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(min.X, min.Y, min.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(max.X, min.Y, min.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(min.X, max.Y, min.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(max.X, max.Y, min.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(min.X, min.Y, max.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(max.X, min.Y, max.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(min.X, max.Y, max.Z)) >= 0) continue;
                if (frustumPlanes[i].GetDistanceToPoint(new Vector3(max.X, max.Y, max.Z)) >= 0) continue;
                return false;
            }
            return true;
        }

        public bool IsSphereInFrustum(Vector3 center, float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                if (frustumPlanes[i].GetDistanceToPoint(center) < -radius)
                {
                    return false;
                }
            }
            return true;
        }

        // ========================================
        // 相机移动
        // ========================================
        public void Move(Vector3 direction, float speed, float deltaTime)
        {
            Position += direction * speed * deltaTime;
        }

        public void MoveForward(float speed, float deltaTime)
        {
            Position += Forward * speed * deltaTime;
        }

        public void MoveRight(float speed, float deltaTime)
        {
            Position += Right * speed * deltaTime;
        }

        public void MoveUp(float speed, float deltaTime)
        {
            Position += Up * speed * deltaTime;
        }

        // ========================================
        // 相机旋转
        // ========================================
        public void Rotate(float yawDelta, float pitchDelta)
        {
            Yaw += yawDelta;
            Pitch += pitchDelta;

            // 限制俯仰角
            Pitch = MathHelper.Clamp(Pitch, GameConstants.PITCH_MIN, GameConstants.PITCH_MAX);

            UpdateCameraVectors();
        }

        public void SetRotation(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = MathHelper.Clamp(pitch, GameConstants.PITCH_MIN, GameConstants.PITCH_MAX);
            UpdateCameraVectors();
        }

        // ========================================
        // 相机抖动
        // ========================================
        public void UpdateBob(float deltaTime, bool isMoving, float speed)
        {
            if (!UseBobEffect)
            {
                bobOffset = 0f;
                return;
            }

            if (isMoving)
            {
                bobTimer += deltaTime * BobSpeed * (speed / GameConstants.PLAYER_WALK_SPEED);
                bobOffset = (float)Math.Sin(bobTimer) * BobAmount * (speed / GameConstants.PLAYER_WALK_SPEED);
            }
            else
            {
                bobOffset = MathHelper.Lerp(bobOffset, 0f, deltaTime * 5f);
            }
        }

        // ========================================
        // 平滑相机更新
        // ========================================
        public void UpdateSmooth(float deltaTime)
        {
            if (!UseSmoothCamera) return;

            float t = 1f - (float)Math.Exp(-SmoothSpeed * deltaTime);
            smoothPosition = Vector3.Lerp(smoothPosition, Position, t);
            smoothYaw = MathHelper.Lerp(smoothYaw, Yaw, t);
            smoothPitch = MathHelper.Lerp(smoothPitch, Pitch, t);
        }

        // ========================================
        // 屏幕坐标转世界射线
        // ========================================
        public Ray ScreenToWorldRay(Vector2 screenPos)
        {
            float ndcX = (2.0f * screenPos.X) / ScreenWidth - 1.0f;
            float ndcY = 1.0f - (2.0f * screenPos.Y) / ScreenHeight;

            Vector4 rayClip = new Vector4(ndcX, ndcY, -1.0f, 1.0f);
            Vector4 rayEye = Vector4.TransformRow(rayClip, InverseProjectionMatrix);
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            Vector4 rayWorld = Vector4.TransformRow(rayEye, InverseViewMatrix);
            Vector3 rayDirection = new Vector3(rayWorld.X, rayWorld.Y, rayWorld.Z).Normalized();

            return new Ray(Position, rayDirection);
        }

        // ========================================
        // 世界坐标转屏幕坐标
        // ========================================
        public Vector2 WorldToScreen(Vector3 worldPos)
        {
            Vector4 clipPos = Vector4.TransformRow(new Vector4(worldPos, 1.0f), ViewProjectionMatrix);

            if (clipPos.W <= 0)
            {
                return new Vector2(-1, -1);
            }

            float ndcX = clipPos.X / clipPos.W;
            float ndcY = clipPos.Y / clipPos.W;

            float screenX = (ndcX + 1.0f) * 0.5f * ScreenWidth;
            float screenY = (1.0f - ndcY) * 0.5f * ScreenHeight;

            return new Vector2(screenX, screenY);
        }

        // ========================================
        // 获取视线方向上的点
        // ========================================
        public Vector3 GetPointOnRay(float distance)
        {
            return Position + Forward * distance;
        }

        // ========================================
        // 重置相机
        // ========================================
        public void Reset()
        {
            Position = Vector3.Zero;
            Yaw = -90f;
            Pitch = 0f;
            Roll = 0f;
            bobTimer = 0f;
            bobOffset = 0f;
            smoothPosition = Vector3.Zero;
            smoothYaw = -90f;
            smoothPitch = 0f;
            UpdateCameraVectors();
            UpdateViewMatrix();
        }

        // ========================================
        // 克隆相机
        // ========================================
        public Camera Clone()
        {
            Camera clone = new Camera(Position, Yaw, Pitch);
            clone.FOV = FOV;
            clone.AspectRatio = AspectRatio;
            clone.NearPlane = NearPlane;
            clone.FarPlane = FarPlane;
            clone.UseBobEffect = UseBobEffect;
            clone.UseSmoothCamera = UseSmoothCamera;
            return clone;
        }
    }

    // ========================================
    // 射线结构体
    // ========================================
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }

        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }

        public bool IntersectsBox(Vector3 min, Vector3 max, out float distance)
        {
            distance = 0f;
            float tmin = 0f;
            float tmax = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                float origin = i == 0 ? Origin.X : i == 1 ? Origin.Y : Origin.Z;
                float dir = i == 0 ? Direction.X : i == 1 ? Direction.Y : Direction.Z;
                float minVal = i == 0 ? min.X : i == 1 ? min.Y : min.Z;
                float maxVal = i == 0 ? max.X : i == 1 ? max.Y : max.Z;

                if (Math.Abs(dir) < 1e-8f)
                {
                    if (origin < minVal || origin > maxVal)
                    {
                        return false;
                    }
                }
                else
                {
                    float t1 = (minVal - origin) / dir;
                    float t2 = (maxVal - origin) / dir;

                    if (t1 > t2)
                    {
                        (t1, t2) = (t2, t1);
                    }

                    tmin = Math.Max(tmin, t1);
                    tmax = Math.Min(tmax, t2);

                    if (tmin > tmax)
                    {
                        return false;
                    }
                }
            }

            distance = tmin;
            return true;
        }
    }

    // ========================================
    // 平面结构体
    // ========================================
    public struct Plane
    {
        public Vector3 Normal;
        public float D;

        public Plane(float a, float b, float c, float d)
        {
            Normal = new Vector3(a, b, c);
            D = d;
        }

        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public void Normalize()
        {
            float length = Normal.Length;
            if (length > 0)
            {
                Normal /= length;
                D /= length;
            }
        }

        public float GetDistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }
    }
}
