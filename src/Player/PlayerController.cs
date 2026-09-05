using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Player
{
    public class PlayerController
    {
        private readonly Camera camera;
        private readonly InputManager input;
        private readonly WorldManager world;
        private readonly PhysicsEngine physics;

        // 玩家状态
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsFlying { get; set; }
        public bool IsSprinting { get; set; }
        public bool IsSneaking { get; set; }
        public bool IsSwimming { get; set; }

        // 玩家属性
        public float Health { get; set; } = 20.0f;
        public float MaxHealth { get; set; } = 20.0f;
        public float Hunger { get; set; } = 20.0f;
        public float MaxHunger { get; set; } = 20.0f;
        public float Saturation { get; set; } = 5.0f;
        public int Experience { get; set; } = 0;
        public int Level { get; set; } = 0;

        // 移动参数
        public float WalkSpeed { get; set; } = 4.317f;
        public float SprintSpeed { get; set; } = 5.612f;
        public float SneakSpeed { get; set; } = 1.295f;
        public float FlySpeed { get; set; } = 10.0f;
        public float SwimSpeed { get; set; } = 2.0f;
        public float JumpForce { get; set; } = 8.0f;
        public float Gravity { get; set; } = -25.0f;
        public float TerminalVelocity { get; set; } = -50.0f;

        // 碰撞盒
        public float PlayerWidth { get; set; } = 0.6f;
        public float PlayerHeight { get; set; } = 1.8f;
        public float EyeHeight { get; set; } = 1.62f;

        // 选中的方块
        public Vector3i? SelectedBlock { get; private set; }
        public Vector3i? SelectedFace { get; private set; }
        public float ReachDistance { get; set; } = 5.0f;

        // 动画
        public float WalkAnimation { get; private set; }
        public float HeadBob { get; private set; }

        public PlayerController(Camera camera, InputManager input, WorldManager world)
        {
            this.camera = camera;
            this.input = input;
            this.world = world;
            physics = new PhysicsEngine(world);
            Position = new Vector3(0, 100, 0);
            Velocity = Vector3.Zero;
        }

        public PlayerController() : this(null, null, null)
        {
        }

        public void Initialize()
        {
            // 找到安全的出生点
            FindSpawnPoint();
            Console.WriteLine("[PlayerController] 玩家控制器初始化完成");
        }

        private void FindSpawnPoint()
        {
            for (int y = GameConstants.CHUNK_HEIGHT - 1; y > 0; y--)
            {
                ushort block = world.GetBlock(0, y, 0);
                if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_WATER_STILL)
                {
                    Position = new Vector3(0.5f, y + 2, 0.5f);
                    break;
                }
            }
        }

        public void Update(float deltaTime)
        {
            // 更新旋转
            UpdateRotation(deltaTime);

            // 更新移动
            UpdateMovement(deltaTime);

            // 更新物理
            UpdatePhysics(deltaTime);

            // 更新相机
            UpdateCamera();

            // 更新选中方块
            UpdateSelectedBlock();

            // 更新动画
            UpdateAnimations(deltaTime);

            // 检查死亡
            if (Position.Y < -10)
            {
                Health = 0;
                Respawn();
            }
        }

        private void UpdateRotation(float deltaTime)
        {
            Vector2 mouseDelta = input.GetMouseDelta();
            float sensitivity = 0.002f;

            Yaw -= mouseDelta.X * sensitivity;
            Pitch -= mouseDelta.Y * sensitivity;

            Pitch = Math.Clamp(Pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);
        }

        private void UpdateMovement(float deltaTime)
        {
            IsSprinting = input.IsKeyDown(Keys.LeftShift) && !IsFlying;
            IsSneaking = input.IsKeyDown(Keys.LeftControl) && !IsFlying;

            Vector3 movement = Vector3.Zero;

            if (input.IsKeyDown(Keys.W)) movement += GetForwardVector();
            if (input.IsKeyDown(Keys.S)) movement -= GetForwardVector();
            if (input.IsKeyDown(Keys.A)) movement -= GetRightVector();
            if (input.IsKeyDown(Keys.D)) movement += GetRightVector();

            if (movement.Length > 0)
            {
                movement.Normalize();

                float speed = IsFlying ? FlySpeed :
                              IsSprinting ? SprintSpeed :
                              IsSneaking ? SneakSpeed :
                              IsSwimming ? SwimSpeed : WalkSpeed;

                Velocity = new Vector3(movement.X * speed, Velocity.Y, movement.Z * speed);
            }
            else
            {
                Velocity = new Vector3(Velocity.X * 0.8f, Velocity.Y, Velocity.Z * 0.8f);
            }

            // 飞行
            if (IsFlying)
            {
                if (input.IsKeyDown(Keys.Space)) Velocity = new Vector3(Velocity.X, FlySpeed, Velocity.Z);
                else if (input.IsKeyDown(Keys.LeftShift)) Velocity = new Vector3(Velocity.X, -FlySpeed, Velocity.Z);
                else Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
            }
            else
            {
                // 跳跃
                if (input.IsKeyPressed(Keys.Space) && IsGrounded)
                {
                    Velocity = new Vector3(Velocity.X, JumpForce, Velocity.Z);
                    IsGrounded = false;
                }

                // 重力
                Velocity = new Vector3(Velocity.X, Velocity.Y + Gravity * deltaTime, Velocity.Z);
                Velocity = new Vector3(Velocity.X, Math.Max(Velocity.Y, TerminalVelocity), Velocity.Z);
            }

            // 切换飞行模式
            if (input.IsKeyPressed(Keys.F))
            {
                IsFlying = !IsFlying;
                Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
            }
        }

        private void UpdatePhysics(float deltaTime)
        {
            Vector3 newPosition = Position + Velocity * deltaTime;

            // 碰撞检测
            newPosition = physics.ResolveCollisions(Position, newPosition, PlayerWidth, PlayerHeight, out bool grounded);
            IsGrounded = grounded;

            if (IsGrounded && Velocity.Y < 0)
            {
                Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
            }

            Position = newPosition;

            // 检查是否在水中
            ushort feetBlock = world.GetBlock((int)Position.X, (int)(Position.Y + 0.1f), (int)Position.Z);
            IsSwimming = feetBlock == GameConstants.BLOCK_WATER_STILL || feetBlock == GameConstants.BLOCK_WATER_FLOWING;
        }

        private void UpdateCamera()
        {
            camera.Position = Position + new Vector3(0, EyeHeight + HeadBob, 0);
            camera.Yaw = Yaw;
            camera.Pitch = Pitch;
            camera.UpdateVectors();
        }

        private void UpdateSelectedBlock()
        {
            SelectedBlock = null;
            SelectedFace = null;

            Vector3 origin = camera.Position;
            Vector3 direction = camera.Front;

            // 射线检测
            for (float t = 0; t < ReachDistance; t += 0.05f)
            {
                Vector3 point = origin + direction * t;
                Vector3i blockPos = new Vector3i(
                    (int)Math.Floor(point.X),
                    (int)Math.Floor(point.Y),
                    (int)Math.Floor(point.Z)
                );

                ushort block = world.GetBlock(blockPos.X, blockPos.Y, blockPos.Z);
                if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_WATER_STILL)
                {
                    SelectedBlock = blockPos;

                    // 计算面法线
                    Vector3 center = blockPos + new Vector3(0.5f);
                    Vector3 diff = point - center;

                    if (Math.Abs(diff.X) > Math.Abs(diff.Y) && Math.Abs(diff.X) > Math.Abs(diff.Z))
                    {
                        SelectedFace = new Vector3i(Math.Sign(diff.X), 0, 0);
                    }
                    else if (Math.Abs(diff.Y) > Math.Abs(diff.X) && Math.Abs(diff.Y) > Math.Abs(diff.Z))
                    {
                        SelectedFace = new Vector3i(0, Math.Sign(diff.Y), 0);
                    }
                    else
                    {
                        SelectedFace = new Vector3i(0, 0, Math.Sign(diff.Z));
                    }

                    break;
                }
            }
        }

        private void UpdateAnimations(float deltaTime)
        {
            float horizontalSpeed = new Vector2(Velocity.X, Velocity.Z).Length;

            if (horizontalSpeed > 0.1f && IsGrounded)
            {
                WalkAnimation += deltaTime * horizontalSpeed * 2;
                HeadBob = MathF.Sin(WalkAnimation) * 0.05f;
            }
            else
            {
                HeadBob *= 0.9f;
            }
        }

        public void BreakSelectedBlock()
        {
            if (SelectedBlock.HasValue)
            {
                Vector3i pos = SelectedBlock.Value;
                ushort block = world.GetBlock(pos.X, pos.Y, pos.Z);

                if (block != GameConstants.BLOCK_BEDROCK)
                {
                    world.SetBlock(pos.X, pos.Y, pos.Z, GameConstants.BLOCK_AIR);
                    world.TriggerBlockBroken(pos, block);
                }
            }
        }

        public void PlaceBlock(ushort blockId)
        {
            if (SelectedBlock.HasValue && SelectedFace.HasValue)
            {
                Vector3i placePos = SelectedBlock.Value + SelectedFace.Value;

                // 检查是否与玩家碰撞
                if (!physics.IsPositionOccupied(placePos, PlayerWidth, PlayerHeight, Position))
                {
                    ushort currentBlock = world.GetBlock(placePos.X, placePos.Y, placePos.Z);
                    if (currentBlock == GameConstants.BLOCK_AIR || currentBlock == GameConstants.BLOCK_WATER_STILL)
                    {
                        world.SetBlock(placePos.X, placePos.Y, placePos.Z, blockId);
                        world.TriggerBlockPlaced(placePos, blockId);
                    }
                }
            }
        }

        public void TakeDamage(float damage, string cause = "generic")
        {
            Health -= damage;
            if (Health <= 0)
            {
                Health = 0;
                Die(cause);
            }
        }

        public void Heal(float amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
        }

        public void AddHunger(float amount)
        {
            Hunger = Math.Min(Hunger + amount, MaxHunger);
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            while (Experience >= GetExperienceForLevel(Level + 1))
            {
                Level++;
            }
        }

        private int GetExperienceForLevel(int level)
        {
            if (level <= 16) return level * level + 6 * level;
            if (level <= 31) return (int)(2.5f * level * level - 40.5f * level + 360);
            return (int)(4.5f * level * level - 162.5f * level + 2220);
        }

        private void Die(string cause)
        {
            Console.WriteLine($"玩家死亡: {cause}");
            Respawn();
        }

        public void Respawn()
        {
            Health = MaxHealth;
            Hunger = MaxHunger;
            Velocity = Vector3.Zero;
            FindSpawnPoint();
        }

        private Vector3 GetForwardVector()
        {
            return new Vector3(
                -MathF.Sin(Yaw),
                0,
                -MathF.Cos(Yaw)
            ).Normalized();
        }

        private Vector3 GetRightVector()
        {
            return new Vector3(
                MathF.Cos(Yaw),
                0,
                -MathF.Sin(Yaw)
            ).Normalized();
        }

        // 扩展属性和方法
        public int ChunkX => (int)Math.Floor(Position.X / 16.0f);
        public int ChunkZ => (int)Math.Floor(Position.Z / 16.0f);
        public int ExperienceLevel { get; set; }
        public GameMode GameMode { get; set; }
        public Items.Inventory Inventory { get; set; }
        public Vector3i? TargetedBlock { get; private set; }

        public Vector3i? GetLookAtBlock()
        {
            return TargetedBlock;
        }

        public Vector3i? Raycast(float maxDistance)
        {
            return TargetedBlock;
        }

        public void ResetForNewWorld()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
    }
}
