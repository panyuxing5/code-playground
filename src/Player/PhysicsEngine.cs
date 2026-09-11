using System;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Player
{
    public class PhysicsEngine
    {
        private readonly WorldManager world;

        // 物理参数
        public float Gravity { get; set; } = -25.0f;
        public float TerminalVelocity { get; set; } = -50.0f;
        public float GroundCheckDistance { get; set; } = 0.01f;
        public float SkinWidth { get; set; } = 0.01f;

        public PhysicsEngine(WorldManager world)
        {
            this.world = world;
        }

        public Vector3 ResolveCollisions(Vector3 oldPosition, Vector3 newPosition, float width, float height, out bool isGrounded)
        {
            isGrounded = false;
            Vector3 result = newPosition;

            // 分轴处理碰撞
            // X轴
            if (Math.Abs(newPosition.X - oldPosition.X) > 0.001f)
            {
                if (CheckCollision(new Vector3(newPosition.X, oldPosition.Y, oldPosition.Z), width, height))
                {
                    result.X = oldPosition.X;
                }
            }

            // Z轴
            if (Math.Abs(newPosition.Z - oldPosition.Z) > 0.001f)
            {
                if (CheckCollision(new Vector3(result.X, oldPosition.Y, newPosition.Z), width, height))
                {
                    result.Z = oldPosition.Z;
                }
            }

            // Y轴
            if (Math.Abs(newPosition.Y - oldPosition.Y) > 0.001f)
            {
                if (CheckCollision(new Vector3(result.X, newPosition.Y, result.Z), width, height))
                {
                    if (newPosition.Y < oldPosition.Y)
                    {
                        // 下落，着地
                        isGrounded = true;
                        // 找到地面高度
                        result.Y = FindGroundHeight(result.X, result.Z, oldPosition.Y, width, height);
                    }
                    else
                    {
                        // 上升，撞头
                        result.Y = oldPosition.Y;
                    }
                }
            }

            // 额外的地面检测
            if (!isGrounded)
            {
                isGrounded = CheckGrounded(result, width, height);
            }

            return result;
        }

        public bool CheckCollision(Vector3 position, float width, float height)
        {
            float halfWidth = width / 2f;

            int minX = (int)Math.Floor(position.X - halfWidth - SkinWidth);
            int maxX = (int)Math.Floor(position.X + halfWidth + SkinWidth);
            int minY = (int)Math.Floor(position.Y - SkinWidth);
            int maxY = (int)Math.Floor(position.Y + height + SkinWidth);
            int minZ = (int)Math.Floor(position.Z - halfWidth - SkinWidth);
            int maxZ = (int)Math.Floor(position.Z + halfWidth + SkinWidth);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        ushort block = world.GetBlock(x, y, z);
                        if (IsSolidBlock(block))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool CheckGrounded(Vector3 position, float width, float height)
        {
            float halfWidth = width / 2f;
            float checkY = position.Y - GroundCheckDistance;

            int minX = (int)Math.Floor(position.X - halfWidth);
            int maxX = (int)Math.Floor(position.X + halfWidth);
            int minZ = (int)Math.Floor(position.Z - halfWidth);
            int maxZ = (int)Math.Floor(position.Z + halfWidth);
            int y = (int)Math.Floor(checkY);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    ushort block = world.GetBlock(x, y, z);
                    if (IsSolidBlock(block))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private float FindGroundHeight(float x, float z, float currentY, float width, float height)
        {
            float halfWidth = width / 2f;

            int minX = (int)Math.Floor(x - halfWidth);
            int maxX = (int)Math.Floor(x + halfWidth);
            int minZ = (int)Math.Floor(z - halfWidth);
            int maxZ = (int)Math.Floor(z + halfWidth);

            float highestGround = float.MinValue;

            for (int bx = minX; bx <= maxX; bx++)
            {
                for (int bz = minZ; bz <= maxZ; bz++)
                {
                    for (int by = (int)Math.Floor(currentY); by >= 0; by--)
                    {
                        ushort block = world.GetBlock(bx, by, bz);
                        if (IsSolidBlock(block))
                        {
                            highestGround = Math.Max(highestGround, by + 1);
                            break;
                        }
                    }
                }
            }

            return highestGround > float.MinValue ? highestGround : currentY;
        }

        public bool IsPositionOccupied(Vector3i blockPos, float width, float height, Vector3 playerPos)
        {
            float halfWidth = width / 2f;

            float blockMinX = blockPos.X;
            float blockMaxX = blockPos.X + 1;
            float blockMinY = blockPos.Y;
            float blockMaxY = blockPos.Y + 1;
            float blockMinZ = blockPos.Z;
            float blockMaxZ = blockPos.Z + 1;

            float playerMinX = playerPos.X - halfWidth;
            float playerMaxX = playerPos.X + halfWidth;
            float playerMinY = playerPos.Y;
            float playerMaxY = playerPos.Y + height;
            float playerMinZ = playerPos.Z - halfWidth;
            float playerMaxZ = playerPos.Z + halfWidth;

            return playerMinX < blockMaxX && playerMaxX > blockMinX &&
                   playerMinY < blockMaxY && playerMaxY > blockMinY &&
                   playerMinZ < blockMaxZ && playerMaxZ > blockMinZ;
        }

        public bool IsSolidBlock(ushort blockId)
        {
            if (blockId == GameConstants.BLOCK_AIR) return false;
            if (blockId == GameConstants.BLOCK_WATER_STILL) return false;
            if (blockId == GameConstants.BLOCK_WATER_FLOWING) return false;
            if (blockId == GameConstants.BLOCK_LAVA_STILL) return false;
            if (blockId == GameConstants.BLOCK_LAVA_FLOWING) return false;
            if (blockId == GameConstants.BLOCK_TORCH) return false;
            if (blockId == GameConstants.BLOCK_WALL_TORCH) return false;
            if (blockId == GameConstants.BLOCK_VINES) return false;
            if (blockId == GameConstants.BLOCK_SNOW) return false;
            if (blockId == GameConstants.BLOCK_GRASS_PLANT) return false;
            if (blockId == GameConstants.BLOCK_FLOWER_RED) return false;
            if (blockId == GameConstants.BLOCK_FLOWER_YELLOW) return false;
            if (blockId == GameConstants.BLOCK_SAPLING) return false;
            if (blockId == GameConstants.BLOCK_SUGAR_CANE) return false;
            if (blockId == GameConstants.BLOCK_CACTUS) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            if (info != null)
            {
                return info.IsSolid;
            }

            return true;
        }

        public bool IsLiquid(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WATER_STILL ||
                   blockId == GameConstants.BLOCK_WATER_FLOWING ||
                   blockId == GameConstants.BLOCK_LAVA_STILL ||
                   blockId == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public float GetLiquidDepth(Vector3 position, float width, float height)
        {
            float halfWidth = width / 2f;
            int x = (int)Math.Floor(position.X);
            int z = (int)Math.Floor(position.Z);

            float depth = 0;
            for (int y = (int)Math.Floor(position.Y); y < position.Y + height; y++)
            {
                ushort block = world.GetBlock(x, y, z);
                if (IsLiquid(block))
                {
                    depth++;
                }
            }

            return depth;
        }

        public Vector3 ApplyGravity(Vector3 velocity, float deltaTime, bool isGrounded)
        {
            if (!isGrounded)
            {
                velocity.Y += Gravity * deltaTime;
                velocity.Y = Math.Max(velocity.Y, TerminalVelocity);
            }
            return velocity;
        }

        public Vector3 ApplyFriction(Vector3 velocity, float friction, float deltaTime)
        {
            velocity.X *= (1.0f - friction * deltaTime);
            velocity.Z *= (1.0f - friction * deltaTime);
            return velocity;
        }

        public Vector3 ApplyAirResistance(Vector3 velocity, float drag, float deltaTime)
        {
            velocity.X *= (1.0f - drag * deltaTime);
            velocity.Z *= (1.0f - drag * deltaTime);
            return velocity;
        }

        public bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, out Vector3 hitPoint, out Vector3i hitBlock, out Vector3i hitNormal)
        {
            hitPoint = Vector3.Zero;
            hitBlock = Vector3i.Zero;
            hitNormal = Vector3i.Zero;

            direction.Normalize();

            // DDA算法
            int x = (int)Math.Floor(origin.X);
            int y = (int)Math.Floor(origin.Y);
            int z = (int)Math.Floor(origin.Z);

            int stepX = direction.X > 0 ? 1 : -1;
            int stepY = direction.Y > 0 ? 1 : -1;
            int stepZ = direction.Z > 0 ? 1 : -1;

            float tDeltaX = direction.X != 0 ? Math.Abs(1.0f / direction.X) : float.MaxValue;
            float tDeltaY = direction.Y != 0 ? Math.Abs(1.0f / direction.Y) : float.MaxValue;
            float tDeltaZ = direction.Z != 0 ? Math.Abs(1.0f / direction.Z) : float.MaxValue;

            float tMaxX = direction.X > 0 ? (x + 1 - origin.X) / direction.X : (origin.X - x) / -direction.X;
            float tMaxY = direction.Y > 0 ? (y + 1 - origin.Y) / direction.Y : (origin.Y - y) / -direction.Y;
            float tMaxZ = direction.Z > 0 ? (z + 1 - origin.Z) / direction.Z : (origin.Z - z) / -direction.Z;

            if (direction.X == 0) tMaxX = float.MaxValue;
            if (direction.Y == 0) tMaxY = float.MaxValue;
            if (direction.Z == 0) tMaxZ = float.MaxValue;

            float t = 0;
            int normalX = 0, normalY = 0, normalZ = 0;

            while (t <= maxDistance)
            {
                ushort block = world.GetBlock(x, y, z);
                if (IsSolidBlock(block))
                {
                    hitPoint = origin + direction * t;
                    hitBlock = new Vector3i(x, y, z);
                    hitNormal = new Vector3i(normalX, normalY, normalZ);
                    return true;
                }

                if (tMaxX < tMaxY && tMaxX < tMaxZ)
                {
                    t = tMaxX;
                    tMaxX += tDeltaX;
                    x += stepX;
                    normalX = -stepX;
                    normalY = 0;
                    normalZ = 0;
                }
                else if (tMaxY < tMaxZ)
                {
                    t = tMaxY;
                    tMaxY += tDeltaY;
                    y += stepY;
                    normalX = 0;
                    normalY = -stepY;
                    normalZ = 0;
                }
                else
                {
                    t = tMaxZ;
                    tMaxZ += tDeltaZ;
                    z += stepZ;
                    normalX = 0;
                    normalY = 0;
                    normalZ = -stepZ;
                }
            }

            return false;
        }
    }
}
